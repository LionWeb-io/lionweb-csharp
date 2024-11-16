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

namespace LionWeb.CSharp.Generator.Names;

using Core.M3;
using Impl;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// <summary>
/// Guarantees unique feature names, renames collisions. 
/// 
/// Relies on <see cref="ClassifierGeneratorBase.FeaturesToImplement"/> to
/// sort implemented interface features before local features. 
/// </summary>
public class UniqueFeatureNames(INames parent) : INames
{
    private readonly Dictionary<Feature, string> _featureNames = [];

    /// <inheritdoc />
    public Language Language => parent.Language;

    /// <inheritdoc />
    public string NamespaceName => parent.NamespaceName;

    /// <inheritdoc />
    public IReadOnlySet<Type> UsedTypes => parent.UsedTypes;

    /// <inheritdoc />
    public IDictionary<Language, string> NamespaceMappings => parent.NamespaceMappings;

    /// <inheritdoc />
    public string LanguageName(Language lang) => parent.LanguageName(lang);

    /// <inheritdoc />
    public NameSyntax LanguageType => parent.LanguageType;

    /// <inheritdoc />
    public string FactoryInterfaceName => parent.FactoryInterfaceName;

    /// <inheritdoc />
    public string FactoryName => parent.FactoryName;

    /// <inheritdoc />
    public IdentifierNameSyntax FactoryInterfaceType => parent.FactoryInterfaceType;

    /// <inheritdoc />
    public IdentifierNameSyntax FactoryType => parent.FactoryType;

    /// <inheritdoc />
    public void AddNamespaceMapping(Language lang, string nsName) =>
        parent.AddNamespaceMapping(lang, nsName);

    /// <inheritdoc />
    public TypeSyntax AsType(Type type, params TypeSyntax?[] generics) => parent.AsType(type, generics);

    /// <inheritdoc />
    public TypeSyntax AsType(Classifier classifier, bool disambiguate = false) =>
        parent.AsType(classifier, disambiguate);

    /// <inheritdoc />
    public TypeSyntax AsType(Datatype datatype, bool disambiguate = false) => parent.AsType(datatype, disambiguate);

    /// <inheritdoc />
    public NameSyntax AsType(Language lang) => parent.AsType(lang);

    /// <inheritdoc />
    public string Use(Type type) => parent.Use(type);

    /// <inheritdoc />
    public IdentifierNameSyntax AsProperty(LanguageEntity classifier) => parent.AsProperty(classifier);

    /// <inheritdoc />
    public IdentifierNameSyntax AsProperty(Feature feature) => parent.AsProperty(feature);

    /// <inheritdoc />
    public IdentifierNameSyntax AsProperty(EnumerationLiteral literal) => parent.AsProperty(literal);

    /// <inheritdoc />
    public IdentifierNameSyntax AsProperty(Field field) => parent.AsProperty(field);

    /// <inheritdoc />
    public NameSyntax MetaProperty(Language lang) => parent.MetaProperty(lang);

    /// <inheritdoc />
    public IdentifierNameSyntax FieldField(Field field) => parent.FieldField(field);

    /// <inheritdoc />
    public IdentifierNameSyntax FieldProperty(Field field) => parent.FieldProperty(field);

    /// <inheritdoc />
    public IdentifierNameSyntax FeatureField(Feature feature)
    {
        RegisterFeatureName(feature);
        return IdentifierName($"_{_featureNames[feature].ToFirstLower()}");
    }

    /// <inheritdoc />
    public IdentifierNameSyntax FeatureProperty(Feature feature)
    {
        RegisterFeatureName(feature);
        return IdentifierName(_featureNames[feature].ToFirstUpper());
    }

    private void RegisterFeatureName(Feature feature)
    {
        if (_featureNames.ContainsKey(feature))
            return;

        var featureName = feature.Name;
        int i = 0;
        while (_featureNames.ContainsValue(featureName))
        {
            featureName = $"{featureName}{i++}";
        }

        _featureNames[feature] = featureName;
    }
}