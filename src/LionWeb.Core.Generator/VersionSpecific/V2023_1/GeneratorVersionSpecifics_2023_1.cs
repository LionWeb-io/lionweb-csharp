// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.Generator.VersionSpecific.V2023_1;

using Core.VersionSpecific.V2023_1;
using M2;
using M3;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// <see cref="GeneratorFacade"/> parts specific to LionWeb <see cref="IVersion2023_1"/>.  
internal class GeneratorVersionSpecifics_2023_1 : IGeneratorVersionSpecifics
{
    public LionWebVersions Version => LionWebVersions.v2023_1;

    public ExpressionSyntax? AsProperty(LanguageEntity entity) => entity switch
    {
        _ when entity.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Node) => ParseExpression("_builtIns.Node"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.INamed) => ParseExpression("_builtIns.INamed"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Boolean) => ParseExpression("_builtIns.Boolean"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Integer) => ParseExpression("_builtIns.Integer"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Json) => ParseExpression("_builtIns.Json"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.String) => ParseExpression("_builtIns.String"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2023_1.Instance.IKeyed) => ParseExpression("_m3.IKeyed"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2023_1.Instance.Classifier) => ParseExpression("_m3.Classifier"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2023_1.Instance.Concept) => ParseExpression("_m3.Concept"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2023_1.Instance.Annotation) => ParseExpression("_m3.Annotation"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2023_1.Instance.Interface) => ParseExpression("_m3.Interface"),
        _ => null
    };

    public TypeSyntax? AsType(Datatype datatype, Dictionary<Language, string> namespaceMappings) => datatype switch
    {
        _ when datatype.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Boolean) =>
            PredefinedType(Token(SyntaxKind.BoolKeyword)),
        _ when datatype.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Integer) =>
            PredefinedType(Token(SyntaxKind.IntKeyword)),
        _ when datatype.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.String) =>
            PredefinedType(Token(SyntaxKind.StringKeyword)),
        _ when datatype.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Json) =>
            PredefinedType(Token(SyntaxKind.StringKeyword)),
        Enumeration when namespaceMappings.TryGetValue(datatype.GetLanguage(), out var ns) =>
            QualifiedName(ParseName(ns), IdentifierName(datatype.Name)),
        _ => null
    };

    public string? GetConceptShortDescription(Classifier classifier) => classifier
        .GetAnnotations()
        .OfType<ConceptDescription>()
        .FirstOrDefault(cd => cd.ConceptShortDescription != null)
        ?.ConceptShortDescription;

    public bool IsDeprecated(Classifier classifier) =>
        classifier.EqualsIdentity(SpecificLanguage.Instance.Deprecated);

    public string? GetDeprecatedComment(IReadableNode annotation) =>
        annotation.Get(SpecificLanguage.Instance.Deprecated_comment) as string;
}