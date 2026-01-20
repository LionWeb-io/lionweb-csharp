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

using Core.M2;
using Core.M3;
using Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

internal class FeatureGeneratorProperty(Classifier classifier, Property property, GeneratorInputParameters generatorInputParameters)
    : FeatureGeneratorBase(classifier, property, generatorInputParameters)
{
    public IEnumerable<MemberDeclarationSyntax> RequiredProperty()
    {
        var setterName = IsValueType()
            ? "SetRequiredValueTypeProperty"
            : "SetRequiredReferenceTypeProperty";

        var members = new List<MemberDeclarationSyntax>
        {
            SingleRequiredFeatureProperty(AsType(property.GetFeatureType())),
            TryGet()
        }.Concat(RequiredFeatureSetter([
            ExpressionStatement(CallGeneric(setterName, AsType(property.Type), IdentifierName("value"), MetaProperty(property), FeatureField(property), FeatureSetRaw(property))),
            ReturnStatement(This())
        ]));
        if (!IsValueType())
            members = members.Select(member => member.Xdoc(XdocThrowsIfSetToNull()));

        return new List<MemberDeclarationSyntax>
        {
            SingleFeatureField(),
            PropertySetterRaw(property.Type)
        }.Concat(members);
    }

    public IEnumerable<MemberDeclarationSyntax> OptionalProperty()
    {
        var setterName = IsValueType()
            ? "SetOptionalValueTypeProperty"
            : "SetOptionalReferenceTypeProperty";

        return new List<MemberDeclarationSyntax>
            {
                SingleFeatureField(),
                SingleOptionalFeatureProperty(),
                TryGet(),
                PropertySetterRaw(property.Type)
            }
            .Concat(
                OptionalFeatureSetter([
                    ExpressionStatement(CallGeneric(setterName, AsType(property.Type), IdentifierName("value"),
                        MetaProperty(property), FeatureField(property), FeatureSetRaw(property))),
                    ReturnStatement(This())
                ])
            );
    }

    private MethodDeclarationSyntax PropertySetterRaw(Datatype datatype)
    {
        ExpressionSyntax comparison = SupportsEqualityOperator(datatype)
            ? EqualsSign(IdentifierName("value"), FeatureField(property))
            : Or(
                And(
                    IsNull("value"),
                    IsNull(FeatureField(property).ToString())
                ),
                And(
                    ParseExpression("value is not null"),
                    InvocationExpression(
                        MemberAccess(
                            IdentifierName("value"),
                            IdentifierName("Equals")
                        )
                    )
                    .WithArgumentList(AsArguments([
                            FeatureField(property)
                        ])
                    )
                )
            );

        var paramType = AsType(datatype);
        return Method(FeatureSetRaw(property).ToString(), AsType(typeof(bool)), [
                Param("value", NullableType(paramType))
            ])
            .WithBody(AsStatements([
                IfStatement(
                    comparison,
                    ReturnStatement(False())
                ),
                AssignFeatureField(),
                ReturnStatement(True())
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));
    }

    private bool SupportsEqualityOperator(Datatype datatype) => datatype switch
    {
        Enumeration or StructuredDataType => true,
        _ when datatype.GetLanguage().EqualsIdentity(_builtIns) => true,
        PrimitiveType p => _names.PrimitiveTypeMappings[p] switch
        {
            { IsPrimitive: true} => true,
            { IsEnum: true } => true,
            { IsValueType: false } => true,
            { } eq when eq.GetMethod(WellKnownMemberNames.EqualityOperatorName) is not null => true,
            _ => false
        },
        _ => false
    };

    private bool IsValueType() => property.Type switch
        {
            Enumeration or StructuredDataType => true,
            PrimitiveType b when _builtIns.Boolean.EqualsIdentity(b) => true,
            PrimitiveType i when _builtIns.Integer.EqualsIdentity(i) => true,
            PrimitiveType s when _builtIns.String.EqualsIdentity(s) => false,
            PrimitiveType p => _names.PrimitiveTypeMappings[p].IsValueType,
            _ => false
        };
}