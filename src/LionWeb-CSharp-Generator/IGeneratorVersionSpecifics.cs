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

namespace LionWeb.CSharp.Generator;

using Core;
using Core.M3;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using VersionSpecific.V2023_1;
using VersionSpecific.V2024_1;

/// Externalized logic of <see cref="GeneratorFacade"/>, specific to one version of LionWeb standard.
internal interface IGeneratorVersionSpecifics : IVersionSpecifics
{
    /// <summary>
    /// Creates an instance of <see cref="IGeneratorVersionSpecifics"/> that implements <paramref name="lionWebVersion"/>.
    /// </summary>
    /// <exception cref="UnsupportedVersionException"></exception>
    static IGeneratorVersionSpecifics Create(LionWebVersions lionWebVersion) => lionWebVersion switch
    {
        IVersion2023_1 => new GeneratorVersionSpecifics_2023_1(),
        IVersion2024_1 => new GeneratorVersionSpecifics_2024_1(),
        IVersion2024_1_Compatible => new GeneratorVersionSpecifics_2024_1_Compatible(),
        _ => throw new UnsupportedVersionException(lionWebVersion)
    };

    /// <inheritdoc cref="INames.AsProperty(LionWeb.Core.M3.LanguageEntity)"/>
    ExpressionSyntax? AsProperty(LanguageEntity entity);

    /// <inheritdoc cref="INames.AsType(System.Type,Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax?[])"/>
    TypeSyntax? AsType(Datatype datatype);
    
    string? GetConceptShortDescription(Classifier classifier);
    bool IsDeprecated(Classifier classifier);
    string? GetDeprecatedComment(IReadableNode annotation);
}