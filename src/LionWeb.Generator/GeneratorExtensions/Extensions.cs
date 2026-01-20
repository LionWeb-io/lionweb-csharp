// Copyright 2026 TRUMPF Laser SE and other contributors
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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// <summary>
/// Extensions to <see cref="GeneratorFacade"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Persists every root-level type in <paramref name="generatorFacade"/>.<see cref="GeneratorFacade.Generate">Generate()</see>
    /// in a separate file named "{typeIdentifier}.g.cs"
    /// in Directory <paramref name="outputDir"/> + appropriate subdir for <paramref name="generatorFacade"/>.Names.NamespaceName.  
    /// </summary>
    /// <param name="generatorFacade"></param>
    /// <param name="outputDir"></param>
    /// <param name="formatted"><see cref="GeneratorFacade.Persist(string, CompilationUnitSyntax, bool)"/></param>
    /// <param name="onPersistedFile">Executed after each persisted file.</param>
    public static void PersistFilePerType(this GeneratorFacade generatorFacade, string outputDir,
        bool formatted = false, Action<CompilationUnitSyntax, string>? onPersistedFile = null)
    {
        var compilationUnit = generatorFacade.Generate();
        var languageNamespace = generatorFacade.Names.NamespaceName;
        foreach (var ns in compilationUnit.Members.OfType<FileScopedNamespaceDeclarationSyntax>())
        {
            foreach (var type in ns.Members.OfType<BaseTypeDeclarationSyntax>())
            {
                var copy = compilationUnit.ReplaceNode(ns,
                    ns.WithMembers(SingletonList<MemberDeclarationSyntax>(type)));
                var namespaces = languageNamespace.Split('.');
                var directory = Path.Combine([outputDir, ..namespaces]);
                Directory.CreateDirectory(directory);
                var file = Path.Combine(directory, $"{type.Identifier.Text}.g.cs");
                generatorFacade.Persist(file, copy, formatted);
                onPersistedFile?.Invoke(copy, file);
            }
        }
    }

    /// <inheritdoc cref="AddUsing(CompilationUnitSyntax, string)"/>
    public static CompilationUnitSyntax AddUsing(this CompilationUnitSyntax compilationUnit, Type type) =>
        compilationUnit.AddUsing(type.Namespace!);

    /// <summary>
    /// Adds "global::<i><paramref name="nameSpace"/></i>" to the <i>usings</i> of the first namespace in <paramref name="compilationUnit"/>.
    /// </summary>
    /// <returns>Modified version of <paramref name="compilationUnit"/>.</returns>
    public static CompilationUnitSyntax AddUsing(this CompilationUnitSyntax compilationUnit, string nameSpace)
    {
        var ns = compilationUnit.Members.OfType<FileScopedNamespaceDeclarationSyntax>().First();
        return compilationUnit.ReplaceNode(
            ns,
            ns.AddUsings(UsingDirective(ParseName($"global::{nameSpace}")))
        );
    }

    /// <summary>
    /// Adds <paramref name="annotations"/> to the initializers in the language constructor in <paramref name="compilationUnit"/>.
    /// </summary>
    /// <returns>Modified version of <paramref name="compilationUnit"/>.</returns>
    public static CompilationUnitSyntax AddM2Annotations(this CompilationUnitSyntax compilationUnit, Correlator correlator,
        Dictionary<IKeyed, CollectionExpressionSyntax> annotations)
    {
        var correlations = annotations.Keys
            .SelectMany(correlator.FindAll<KeyedToLanguageConstructorCorrelation>);

        foreach (var correlation in correlations)
        {
            var assignment = correlation.LookupIn(compilationUnit);
            if (assignment.Right is ImplicitObjectCreationExpressionSyntax
                {
                    ArgumentList.Arguments:
                    [
                        {
                            Expression: ParenthesizedLambdaExpressionSyntax
                            {
                                Body: ObjectCreationExpressionSyntax
                                {
                                    Initializer: {} init
                                }
                            }
                        }
                    ]
                })
            {
                compilationUnit = compilationUnit.ReplaceNode(
                    init,
                    init.AddExpressions(AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName("Annotations"),
                        annotations[correlation.Keyed]
                    ))
                );
            }
        }

        return compilationUnit;
    }
}