﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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
// SPDX-FileCopyrightText: 2025 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

// ReSharper disable InconsistentNaming

namespace LionWeb.Generator.VersionSpecific.V2025_1;

using Core;
using Core.M2;
using Core.M3;
using Core.Utilities;
using Core.VersionSpecific.V2025_1;
using Io.Lionweb.Mps.Specific.V2025_1;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// <see cref="GeneratorFacade"/> parts specific to LionWeb <see cref="IVersion2025_1"/>.  
internal class GeneratorVersionSpecifics_2025_1 : IGeneratorVersionSpecifics
{
    public virtual LionWebVersions Version => LionWebVersions.v2025_1;

    public virtual ExpressionSyntax? AsProperty(LanguageEntity entity) => entity switch
    {
        _ when entity.EqualsIdentity(BuiltInsLanguage_2025_1.Instance.Node) => ParseExpression("_builtIns.Node"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2025_1.Instance.INamed) => ParseExpression("_builtIns.INamed"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2025_1.Instance.Boolean) => ParseExpression("_builtIns.Boolean"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2025_1.Instance.Integer) => ParseExpression("_builtIns.Integer"),
        _ when entity.EqualsIdentity(BuiltInsLanguage_2025_1.Instance.String) => ParseExpression("_builtIns.String"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2025_1.Instance.IKeyed) => ParseExpression("_m3.IKeyed"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2025_1.Instance.Classifier) => ParseExpression("_m3.Classifier"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2025_1.Instance.Concept) => ParseExpression("_m3.Concept"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2025_1.Instance.Annotation) => ParseExpression("_m3.Annotation"),
        _ when entity.EqualsIdentity(LionCoreLanguage_2025_1.Instance.Interface) => ParseExpression("_m3.Interface"),
        _ => null
    };

    public virtual TypeSyntax? AsType(Datatype datatype, Dictionary<Language, string> namespaceMappings) => datatype switch
    {
        _ when datatype.EqualsIdentity(BuiltInsLanguage_2025_1.Instance.Boolean) =>
            PredefinedType(Token(SyntaxKind.BoolKeyword)),
        _ when datatype.EqualsIdentity(BuiltInsLanguage_2025_1.Instance.Integer) =>
            PredefinedType(Token(SyntaxKind.IntKeyword)),
        _ when datatype.EqualsIdentity(BuiltInsLanguage_2025_1.Instance.String) =>
            PredefinedType(Token(SyntaxKind.StringKeyword)),
        Enumeration or StructuredDataType when namespaceMappings.TryGetValue(datatype.GetLanguage(), out var ns) =>
            QualifiedName(ParseName(ns), IdentifierName(datatype.Name)),
        _ => null
    };
    
    public string? GetConceptShortDescription(Classifier classifier) => 
        GetConceptDescription(classifier)
            ?.ConceptShortDescription;

    public string? GetConceptHelpUrl(Classifier classifier) =>
        GetConceptDescription(classifier)
            ?.HelpUrl;

    public string? GetKeyedDocumentation(IKeyed keyed) =>
        GetKeyedDescription(keyed)
            ?.Documentation;

    public IReadOnlyList<IReadableNode> GetKeyedSeeAlso(IKeyed keyed) =>
        GetKeyedDescription(keyed)
            ?.SeeAlso ?? [];

    private static ConceptDescription? GetConceptDescription(Classifier classifier) =>
        classifier
            .GetAnnotations()
            .OfType<ConceptDescription>()
            .FirstOrDefault();

    private static KeyedDescription? GetKeyedDescription(IKeyed keyed) =>
        keyed
            .GetAnnotations()
            .OfType<KeyedDescription>()
            .FirstOrDefault();

    public bool IsDeprecated(Classifier classifier) =>
        classifier.EqualsIdentity(SpecificLanguage.Instance.Deprecated);

    public string? GetDeprecatedComment(IReadableNode annotation) =>
        annotation.Get(SpecificLanguage.Instance.Deprecated_comment) as string;
}