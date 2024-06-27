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

/// <summary>
/// Common base class for all generators for the language class.
/// </summary>
public abstract class LanguageGeneratorBase : GeneratorBase
{
    /// <inheritdoc cref="LanguageGeneratorBase"/>
    protected LanguageGeneratorBase(INames names) : base(names) { }

    /// <inheritdoc cref="INames.LanguageName"/>
    protected string LanguageName => _names.LanguageName(_names.Language);

    /// <returns><c>_myEntity</c></returns>
    protected string LanguageFieldName(LanguageEntity entity) =>
        $"_{entity.Name.ToFirstLower()}";

    /// <returns><c>_myClassifier_MyFeature</c></returns>
    protected string LanguageFieldName(Feature feature) =>
        $"_{feature.GetFeatureClassifier().Name.ToFirstLower()}_{feature.Name}";

    /// <returns><c>myEnum_MyEnumLiteral</c></returns>
    protected string LanguageFieldName(EnumerationLiteral literal) =>
        $"_{literal.GetEnumeration().Name.ToFirstLower()}_{literal.Name}";

    /// Returns FQN if <paramref name="entity">entity's</paramref> language is part of <see cref="INames.NamespaceMappings"/>.
    /// <inheritdoc cref="INames.AsProperty(LionWeb.Core.M3.LanguageEntity)"/>
    protected ExpressionSyntax AsProperty(LanguageEntity entity)
    {
        if (entity.EqualsIdentity(BuiltInsLanguage.Instance.Node))
            return ParseExpression("BuiltInsLanguage.Instance.Node");
        if (entity.EqualsIdentity(BuiltInsLanguage.Instance.INamed))
            return ParseExpression("BuiltInsLanguage.Instance.INamed");
        if (entity.EqualsIdentity(BuiltInsLanguage.Instance.Boolean))
            return ParseExpression("BuiltInsLanguage.Instance.Boolean");
        if (entity.EqualsIdentity(BuiltInsLanguage.Instance.Integer))
            return ParseExpression("BuiltInsLanguage.Instance.Integer");
        if (entity.EqualsIdentity(BuiltInsLanguage.Instance.Json))
            return ParseExpression("BuiltInsLanguage.Instance.Json");
        if (entity.EqualsIdentity(BuiltInsLanguage.Instance.String))
            return ParseExpression("BuiltInsLanguage.Instance.String");

        if (_names.NamespaceMappings.ContainsKey(entity.GetLanguage()))
        {
            return
                QualifiedName(
                    _names.MetaProperty(entity.GetLanguage()),
                    IdentifierName(entity.Name)
                );
        }

        return _names.AsProperty(entity);
    }

    /// <inheritdoc cref="INames.AsProperty(LionWeb.Core.M3.EnumerationLiteral)"/>
    protected IdentifierNameSyntax AsProperty(EnumerationLiteral literal) =>
        _names.AsProperty(literal);
}