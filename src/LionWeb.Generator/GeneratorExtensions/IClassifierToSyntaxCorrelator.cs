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

namespace LionWeb.Generator.GeneratorExtensions;

using Core.M3;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Interface for correlating LionWeb's <see cref="Classifier"/>s to their corresponding generated C# syntax nodes.
/// </summary>
public interface IClassifierToSyntaxCorrelator
{
    /// <inheritdoc cref="Classifier"/>
    Classifier Classifier { get; }
}

/// <summary>
/// Record for holding <see cref="ClassDeclarationSyntax"/> of <see cref="Concept"/> and <see cref="Annotation"/> type of classifier.
/// </summary>
public record ClassSyntax(
    Classifier ClassifierType,
    ClassDeclarationSyntax ClassDeclarationSyntax) : IClassifierToSyntaxCorrelator
{
    /// <inheritdoc />
    public Classifier Classifier => ClassifierType;
}

/// <summary>
/// Record for holding <see cref="InterfaceDeclarationSyntax"/> of <see cref="Interface"/> type of classifier.
/// </summary>
public record InterfaceSyntax(
    Classifier ClassifierType,
    InterfaceDeclarationSyntax InterfaceDeclarationSyntax) : IClassifierToSyntaxCorrelator
{
    /// <inheritdoc />
    public Classifier Classifier => ClassifierType;
}