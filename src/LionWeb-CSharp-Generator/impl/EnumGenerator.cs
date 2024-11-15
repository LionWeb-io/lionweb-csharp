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

using Core.M3;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

/// <summary>
/// Generates Enumeration enums.
/// </summary>
public class EnumGenerator(Enumeration enumeration, INames names) : GeneratorBase(names)
{
    /// <inheritdoc cref="EnumGenerator"/>
    public EnumDeclarationSyntax EnumType() =>
        EnumDeclaration(enumeration.Name)
            .WithAttributeLists(AsAttributes(
                [
                    MetaPointerAttribute(enumeration),
                    ObsoleteAttribute(enumeration)
                ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithMembers(SeparatedList(enumeration.Literals.Select(Literal)));

    private EnumMemberDeclarationSyntax Literal(EnumerationLiteral literal) =>
        EnumMember(literal.Name)
            .WithAttributeLists(AsAttributes([MetaPointerAttribute(literal)]));
}