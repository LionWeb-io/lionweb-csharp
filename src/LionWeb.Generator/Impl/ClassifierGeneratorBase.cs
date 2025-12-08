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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace LionWeb.Generator.Impl;

using Core;
using Core.M2;
using Core.M3;
using Core.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

/// <summary>
/// Common base class for all generators for concept/annotation classes and interface interfaces.
/// </summary>
public abstract class ClassifierGeneratorBase(INames names, LionWebVersions lionWebVersion, GeneratorConfig config)
    : GeneratorBase(names, lionWebVersion, config)
{
    /// <summary>
    /// Required by <see cref="UniqueFeatureNames"/> to 
    /// sort implemented interface features before local features. 
    /// </summary>
    protected IEnumerable<Feature> FeaturesToImplement(Classifier classifier) =>
        InterfaceFeatures(classifier)
            .Concat(classifier.Features)
            .Ordered();

    /// Returns features directly inherited from an interface,
    /// and not yet implemented by <paramref name="classifier">classifier's</paramref> super-classifier.
    private IEnumerable<Feature> InterfaceFeatures(Classifier classifier) =>
        classifier
            .DirectGeneralizations()
            .OfType<Interface>()
            .SelectMany(i => i.AllGeneralizations(true))
            .SelectMany(i => i.Features)
            .Distinct(new FeatureIdentityComparer())
            .Except(classifier
                .AllGeneralizations()
                .Where(c => c is Concept or Annotation)
                .SelectMany(InterfaceFeatures)
            );

    protected ExpressionStatementSyntax AssureNotNullCall(Link link) =>
        ExpressionStatement(Call("AssureNotNull",
            IdentifierName("safeNodes"),
            MetaProperty(link)
        ));

    protected ExpressionStatementSyntax AssureNotNullMembersCall(Link link) =>
        ExpressionStatement(Call("AssureNotNullMembers",
            IdentifierName("safeNodes"),
            MetaProperty(link)
        ));

    protected ExpressionStatementSyntax EmitterCollectOldDataCall() =>
        ExpressionStatement(
            InvocationExpression(MemberAccess(IdentifierName("emitter"), IdentifierName("CollectOldData"))));

    protected ExpressionStatementSyntax EmitterNotifyCall() =>
        ExpressionStatement(InvocationExpression(MemberAccess(IdentifierName("emitter"), IdentifierName("Notify"))));

    protected ExpressionSyntax FeatureSetRaw(Feature feature) =>
        IdentifierName(FeatureSet(feature) + "Raw");
}