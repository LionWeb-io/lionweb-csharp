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

public class FeatureGeneratorProperty(Classifier classifier, Property property, INames names, LionWebVersions lionWebVersion, GeneratorConfig config) : FeatureGeneratorBase(classifier, property, names, lionWebVersion, config)
{
    public IEnumerable<MemberDeclarationSyntax> RequiredProperty()
    {
        var setterName = IsReferenceType()
            ? "SetRequiredReferenceTypeProperty"
            : "SetRequiredValueTypeProperty";

        var members = new List<MemberDeclarationSyntax>
        {
            SingleRequiredFeatureProperty(AsType(property.GetFeatureType())),
            TryGet()
        }.Concat(RequiredFeatureSetter([
            ExpressionStatement(CallGeneric(setterName, AsType(property.Type), IdentifierName("value"), MetaProperty(property), FeatureField(property), FeatureSetRaw(property))),
            ReturnStatement(This())
        ]));
        if (IsReferenceType())
            members = members.Select(member => member.Xdoc(XdocThrowsIfSetToNull()));

        return new List<MemberDeclarationSyntax>
        {
            SingleFeatureField(),
            FeatureSetterRaw(AsType(property.Type))
        }.Concat(members);
    }

    public IEnumerable<MemberDeclarationSyntax> OptionalProperty()
    {
        var setterName = IsReferenceType()
            ? "SetOptionalReferenceTypeProperty"
            : "SetOptionalValueTypeProperty";

        return new List<MemberDeclarationSyntax>
            {
                SingleFeatureField(),
                SingleOptionalFeatureProperty(),
                TryGet(),
                FeatureSetterRaw(AsType(property.Type))
            }
            .Concat(
                OptionalFeatureSetter([
                    ExpressionStatement(CallGeneric(setterName, AsType(property.Type), IdentifierName("value"),
                        MetaProperty(property), FeatureField(property), FeatureSetRaw(property))),
                    ReturnStatement(This())
                ])
            );
    }

    private bool IsReferenceType() =>
        !(_builtIns.Boolean.EqualsIdentity(property.Type)
          || _builtIns.Integer.EqualsIdentity(property.Type)
          || property.Type is Enumeration
          || property.Type is StructuredDataType
            );
}