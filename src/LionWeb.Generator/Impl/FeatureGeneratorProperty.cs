// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Generator.Impl;

using Core;
using Core.M2;
using Core.M3;
using Core.Notification.Partition.Emitter;
using Core.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

internal class FeatureGeneratorProperty(Classifier classifier, Property property, INames names, LionWebVersions lionWebVersion, GeneratorConfig config) : FeatureGeneratorBase(classifier, property, names, lionWebVersion, config)
{
    public IEnumerable<MemberDeclarationSyntax> RequiredProperty()
    {
        List<StatementSyntax> setterBody =
        [
            PropertyEmitterVariable(),
            EmitterCollectOldDataCall(),
            AssignFeatureField(),
            EmitterNotifyCall(),
            ReturnStatement(This())
        ];
        if (IsReferenceType(property))
            setterBody.Insert(0, AsureNotNullCall());

        var prop = SingleRequiredFeatureProperty(AsType(property.GetFeatureType()));
        var setter = RequiredFeatureSetter(setterBody);

        var members = new List<MemberDeclarationSyntax> { prop, TryGet() }.Concat(setter);
        if (IsReferenceType(property))
            members = members.Select(member => member.Xdoc(XdocThrowsIfSetToNull()));

        return new List<MemberDeclarationSyntax> { SingleFeatureField() }.Concat(members);
    }

    public IEnumerable<MemberDeclarationSyntax> OptionalProperty() =>
        new List<MemberDeclarationSyntax>
            {
                SingleFeatureField(),
                SingleOptionalFeatureProperty(),
                TryGet()
            }
            .Concat(
                OptionalFeatureSetter([
                    PropertyEmitterVariable(),
                    EmitterCollectOldDataCall(),
                    AssignFeatureField(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
            );

    private LocalDeclarationStatementSyntax PropertyEmitterVariable() =>
        Variable(
            "emitter",
            AsType(typeof(PropertyNotificationEmitter)),
            NewCall([
                MetaProperty(property), This(), IdentifierName("value"),
                FeatureField(property), IdentifierName("notificationId")
            ])
        );

    private bool IsReferenceType(Property property) =>
        !(_builtIns.Boolean.EqualsIdentity(property.Type) ||
          _builtIns.Integer.EqualsIdentity(property.Type));
}