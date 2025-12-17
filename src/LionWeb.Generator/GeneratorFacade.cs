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

namespace LionWeb.Generator;

using Core;
using GeneratorExtensions;
using Impl;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Names;
using System.Collections.Immutable;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// Public API for LionWeb C# generator.
/// Creates one file containing
/// <ul>
/// <li><c>MyLangNameLanguage</c> class</li>
/// <li><c>MyLangNameFactory</c> class</li>
/// <li>One class per Annotation or Concept</li>
/// <li>One C# interface per LionWeb Interface</li>
/// <li>One C# enum per LionWeb Enumeration</li> 
/// </ul>
/// Source is <see cref="Names">INames'</see> <see cref="INames.Language"/> with target namespace <see cref="INames.NamespaceName"/>.
public class GeneratorFacade
{
    private CompilationUnitSyntax? _compilationUnit = null;

    /// Generates the compilation unit for the input language.
    public CompilationUnitSyntax Generate()
    {
        if (_compilationUnit == null)
        {
            var generatorInputParameters = new GeneratorInputParameters
            {
                Names = Names, LionWebVersion = LionWebVersion, Config = Config, Correlator = Correlator
            };

            var generator = new DefinitionGenerator(generatorInputParameters);
            _compilationUnit = generator.DefinitionFile();
        }

        return _compilationUnit;
    }

    /// <inheritdoc cref="INames"/>
    public required INames Names { get; init; }

    /// Version of LionWeb standard to use for generation.
    public LionWebVersions LionWebVersion { get; init; } = LionWebVersions.Current;

    /// <inheritdoc cref="GeneratorConfig"/>
    public GeneratorConfig Config { get; init; } = new();

    /// <inheritdoc cref="Correlator"/>
    public Correlator Correlator { get; } = new();

    /// Stores the output of <see cref="Generate"/> to the file at <paramref name="path"/>.
    public void Persist(string path) =>
        Persist(path, Generate());

    /// Stores the output of <paramref name="compilationUnit"/> to the file at <paramref name="path"/>.
    public void Persist(string path, CompilationUnitSyntax compilationUnit)
    {
        var workspace = new AdhocWorkspace();
        var options = workspace.Options
            .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, value: true)
            .WithChangedOption(FormattingOptions.SmartIndent, LanguageNames.CSharp,
                value: FormattingOptions.IndentStyle.Smart)
            .WithChangedOption(CSharpFormattingOptions.WrappingKeepStatementsOnSingleLine, value: false)
            .WithChangedOption(CSharpFormattingOptions.NewLineForMembersInAnonymousTypes, value: true)
            .WithChangedOption(CSharpFormattingOptions.NewLineForMembersInObjectInit, value: true);
        var formattedCompilationUnit = (CompilationUnitSyntax)Formatter.Format(compilationUnit, workspace, options);

        using var streamWriter = new StreamWriter(path, false);
        streamWriter.Write(formattedCompilationUnit.GetText().ToString().ReplaceLineEndings());
        /*
         * Note: .GetText().ToString() probably nullifies any optimization gains through streaming,
         * but it's the only way (that I found) to actually replace line endings.
         * (E.g., setting the option (FormattingOptions.NewLine, LanguageNames.CSharp, value: "\n"))
         *  – or any variation of that – does nothing!)
         */
    }

    /// Compiles the output of <see cref="Generate"/> and returns all diagnostic messages.
    public ImmutableArray<Diagnostic> Compile() =>
        Compile(Generate());

    /// Compiles <paramref name="compilationUnit"/> and returns all diagnostic messages.
    public ImmutableArray<Diagnostic> Compile(CompilationUnitSyntax compilationUnit)
    {
        var tree = SyntaxTree(compilationUnit);
        var refApis = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Append(MetadataReference.CreateFromFile(typeof(Stack<>).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(ISet<>).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
        var compilation = CSharpCompilation.Create("foo", [tree], refApis);
        var diagnostics = compilation.GetDiagnostics();
        return diagnostics;
    }
}