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

namespace LionWeb.CSharp.Generator.Impl;

using Core.M2;
using Core.M3;
using Core.Utilities;
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

    /// <inheritdoc cref="GeneratorBase"/>
    protected GeneratorBase(INames names)
    {
        _names = names;
    }

    /// <inheritdoc cref="INames.Language"/>
    protected Language Language => _names.Language;

    /// <inheritdoc cref="INames.LanguageType"/>
    protected NameSyntax LanguageType => _names.LanguageType;

    /// <inheritdoc cref="INames.AsType(System.Type,Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax?[])"/>
    protected TypeSyntax AsType(Type type, params TypeSyntax?[] generics) =>
        _names.AsType(type, generics);

    /// Roslyn type of <paramref name="entity"/> (also registered with <see cref="INames.UsedTypes"/>).
    /// Returns FQN if <paramref name="disambiguate"/> is <c>true</c>.
    protected TypeSyntax AsType(LanguageEntity entity, bool disambiguate = false) =>
        entity switch
        {
            Classifier c => AsType(c, disambiguate),
            Datatype d => _names.AsType(d, disambiguate),
            _ => throw new ArgumentException($"unsupported entity: {entity}", nameof(entity))
        };

    /// <inheritdoc cref="INames.AsType(Classifier, bool)"/>
    protected TypeSyntax AsType(Classifier classifier, bool disambiguate = false) =>
        _names.AsType(classifier, disambiguate);

    /// <returns><c>AddMyLink</c></returns>
    protected ExpressionSyntax LinkAdd(Link link) =>
        IdentifierName($"Add{link.Name.ToFirstUpper()}");

    /// <inheritdoc cref="INames.FeatureField"/>
    protected ExpressionSyntax FeatureField(Feature feature) =>
        _names.FeatureField(feature);

    /// <inheritdoc cref="INames.FeatureProperty"/>
    protected ExpressionSyntax FeatureProperty(Feature feature) =>
        _names.FeatureProperty(feature);

    /// <returns><c>MyLang.Instance.MyClassifier_MyFeature</c></returns>
    protected MemberAccessExpressionSyntax MetaProperty(Feature feature)
    {
        if (BuiltInsLanguage.Instance.INamed_name.EqualsIdentity(feature))
            return (MemberAccessExpressionSyntax)ParseExpression("BuiltInsLanguage.Instance.INamed_name");

        return MemberAccess(_names.MetaProperty(feature.GetLanguage()), _names.AsProperty(feature));
    }

    /// <returns><c>[LionCoreMetaPointer(Language = typeof(MyLangNameLanguage), Key = "keyedKey")]</c></returns>
    protected AttributeSyntax MetaPointerAttribute(IKeyed keyed)
    {
        var languageName = keyed.GetLanguage() switch
        {
            var l when l.EqualsIdentity(BuiltInsLanguage.Instance) => nameof(BuiltInsLanguage),
            var l when l.EqualsIdentity(M3Language.Instance) => nameof(M3Language),
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
}