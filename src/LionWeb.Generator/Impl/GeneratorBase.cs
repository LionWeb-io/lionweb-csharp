// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Generator.Impl;

using Core;
using Core.M2;
using Core.M3;
using Core.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

/// <summary>
/// Common base class for all generators.
/// </summary>
public abstract class GeneratorBase
{
    /// <inheritdoc cref="INames"/>
    protected readonly INames _names;

    protected readonly LionWebVersions _lionWebVersion;
    protected readonly GeneratorConfig _config;
    protected readonly ILionCoreLanguage _m3;
    protected readonly IBuiltInsLanguage _builtIns;

    /// <inheritdoc cref="GeneratorBase"/>
    protected GeneratorBase(INames names, LionWebVersions lionWebVersion, GeneratorConfig config)
    {
        lionWebVersion.AssureCompatible(names.Language.LionWebVersion);
        _names = names;
        _lionWebVersion = lionWebVersion;
        _config = config;
        _m3 = lionWebVersion.LionCore;
        _builtIns = lionWebVersion.BuiltIns;
    }

    /// <inheritdoc cref="INames.Language"/>
    protected Language Language => _names.Language;

    /// <inheritdoc cref="INames.LanguageType"/>
    protected NameSyntax LanguageType => _names.LanguageType;

    /// <inheritdoc cref="IGeneratorVersionSpecifics"/>
    internal IGeneratorVersionSpecifics VersionSpecifics =>
        new Lazy<IGeneratorVersionSpecifics>(() => IGeneratorVersionSpecifics.Create(_lionWebVersion)).Value;

    /// <inheritdoc cref="INames.AsType(System.Type,Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax?[])"/>
    protected TypeSyntax AsType(Type type, params TypeSyntax?[] generics) =>
        _names.AsType(type, generics);

    /// Roslyn type of <paramref name="entity"/> (also registered with <see cref="INames.UsedTypes"/>).
    /// Returns FQN if <paramref name="disambiguate"/> is <c>true</c>.
    protected TypeSyntax AsType(LanguageEntity entity, bool disambiguate = false, bool writeable = false) =>
        entity switch
        {
            Classifier c => AsType(c, disambiguate, writeable),
            Datatype d => _names.AsType(d, disambiguate),
            _ => throw new ArgumentException($"unsupported entity: {entity}", nameof(entity))
        };

    /// <inheritdoc cref="INames.AsType(Classifier, bool, bool)"/>
    protected TypeSyntax AsType(Classifier classifier, bool disambiguate = false, bool writeable = false) =>
        _names.AsType(classifier, disambiguate, writeable);

    /// <returns><c>AddMyLink</c></returns>
    protected ExpressionSyntax LinkAdd(Link link) =>
        IdentifierName($"Add{link.Name.ToFirstUpper()}");

    /// <inheritdoc cref="INames.FeatureField"/>
    protected ExpressionSyntax FeatureField(Feature feature) =>
        _names.FeatureField(feature);

    /// <inheritdoc cref="INames.FeatureProperty"/>
    protected ExpressionSyntax FeatureProperty(Feature feature) =>
        _names.FeatureProperty(feature);

    /// <inheritdoc cref="INames.FieldField"/>
    protected ExpressionSyntax FieldField(Field field) =>
        _names.FieldField(field);

    /// <inheritdoc cref="INames.FieldProperty"/>
    protected ExpressionSyntax FieldProperty(Field field) =>
        _names.FieldProperty(field);

    /// <returns><c>MyLang.Instance.MyClassifier_MyFeature</c></returns>
    protected MemberAccessExpressionSyntax MetaProperty(Feature feature)
    {
        if (_builtIns.INamed_name.EqualsIdentity(feature))
            return (MemberAccessExpressionSyntax)ParseExpression("_builtIns.INamed_name");

        return MemberAccess(_names.MetaProperty(feature.GetLanguage()), _names.AsProperty(feature));
    }

    /// <returns><c>MyLang.Instance.MyStructuredDataType_MyField</c></returns>
    protected MemberAccessExpressionSyntax MetaProperty(Field field) =>
        MemberAccess(_names.MetaProperty(field.GetLanguage()), _names.AsProperty(field));

    /// <returns><code>
    /// /// keyed.@KeyedDescription.documentation
    /// /// &lt;seealso cref="keyed.@KeyedDescription.seeAlso"/&gt;
    /// </code></returns>
    protected List<XmlNodeSyntax> XdocKeyed(IKeyed keyed)
    {
        List<XmlNodeSyntax> result = [];
        
        var keyedDocumentation = VersionSpecifics.GetKeyedDocumentation(keyed);
        if (keyedDocumentation != null)
            result.AddRange(XdocLine(keyedDocumentation));

        var keyedSeeAlso = VersionSpecifics.GetKeyedSeeAlso(keyed).OfType<IKeyed>().ToList();
        if (keyedSeeAlso.Count != 0)
            result.AddRange(keyedSeeAlso.Select(k => _names.AsName(k)).SelectMany(XdocSeeAlso));

        return result;
    }

    /// <returns><c>[LionCoreMetaPointer(Language = typeof(MyLangNameLanguage), Key = "keyedKey")]</c></returns>
    protected AttributeSyntax MetaPointerAttribute(IKeyed keyed)
    {
        var languageName = keyed.GetLanguage() switch
        {
            var l when l.EqualsIdentity(_builtIns) => _builtIns.GetType().FullName,
            var l when l.EqualsIdentity(_m3) => _m3.GetType().FullName,
            var l => _names.AsType(l).ToString()
        };

        return AsAttribute(
            AsType(typeof(LionCoreMetaPointer)),
            [
                ("Language", TypeOfExpression(IdentifierName(languageName))),
                ("Key", keyed.Key.AsLiteral())
            ]
        );
    }

    /// <returns><c>[Obsolete(Message = "comment")]</c></returns>
    protected AttributeSyntax? ObsoleteAttribute(IKeyed keyed)
    {
        var deprecatedAnnotation = keyed
            .GetAnnotations()
            .FirstOrDefault(a => VersionSpecifics.IsDeprecated(a.GetClassifier()));
        if (deprecatedAnnotation == null)
            return null;

        var result = Attribute(IdentifierName("Obsolete"));

        var comment = VersionSpecifics.GetDeprecatedComment(deprecatedAnnotation);
        if (comment is not string s)
            return result;

        return result
            .WithArgumentList(AttributeArgumentList(SingletonSeparatedList(
                AttributeArgument(s.AsLiteral())
            )));
    }
    
    /// <returns><c>TryGetMyFeature</c></returns>
    protected string FeatureTryGet(Feature feature) =>
        $"TryGet{feature.Name.ToFirstUpper()}";
    
    /// <returns><c>SetMyFeature</c></returns>
    protected string FeatureSet(Feature feature) =>
        $"Set{feature.Name.ToFirstUpper()}";
}