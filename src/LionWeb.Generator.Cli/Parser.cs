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

namespace LionWeb.Generator.Cli;

using Core;
using System.CommandLine;
using System.Text.Json;

internal class Parser
{
    private readonly Action<string> _errorLogger;
    private readonly string[] _args;

    private readonly Option<FileInfo?> _config;
    private readonly Argument<FileInfo[]> _languageFile;
    private readonly RootCommand _rootCommand;
    private readonly Option<DirectoryInfo> _outputDir;
    private readonly Option<FileInfo?> _outputFile;
    private readonly Option<string?> _ns;
    private readonly Option<NamespacePattern?> _namespacePattern;
    private readonly Option<PathPattern?> _pathPattern;
    private readonly Option<string?> _lionWebVersion;
    private readonly Option<bool?> _writableIface;
    private ParseResult? _parseResult;

    public List<Configuration> Configurations { get; } = [];

    private ParseResult ParseResult => _parseResult ??= _rootCommand.Parse(_args);

    public Parser(Action<string> errorLogger, string[] args)
    {
        _errorLogger = errorLogger;
        _args = args;

        _rootCommand = new("Generates C# classes from LionWeb languages");

        _config = new("--config") { Description = "Configuration file", Arity = ArgumentArity.ZeroOrOne };
        _config.AcceptLegalFilePathsOnly();
        _config.AcceptExistingOnly();
        _rootCommand.Options.Add(_config);

        _outputDir = new("--output")
        {
            Description = "Directory to write generated files to", Arity = ArgumentArity.ZeroOrOne
        };
        _outputDir.AcceptLegalFilePathsOnly();
        _rootCommand.Options.Add(_outputDir);

        _outputFile = new("--outputFile")
        {
            Description = "File to write generated language to", Arity = ArgumentArity.ZeroOrOne
        };
        _outputFile.AcceptLegalFilePathsOnly();
        _rootCommand.Options.Add(_outputFile);

        _ns = new("--namespace")
        {
            Description = "Namespace to generate languages into", Arity = ArgumentArity.ZeroOrOne
        };
        _ns.Validators.Add(s =>
            {
                var value = s.GetValue(_ns);
                if (value is not null && value.Length > 0 &&
                    !NamespaceUtil.NamespaceRegex().IsMatch(value))
                    s.AddError($"Not a valid namespace: {value}");
            }
        );
        _rootCommand.Options.Add(_ns);

        _namespacePattern = new("--namespacePattern")
        {
            Description = "Pattern for namespace to generate languages into", Arity = ArgumentArity.ZeroOrOne
        };
        _rootCommand.Options.Add(_namespacePattern);

        _pathPattern = new("--pathPattern")
        {
            Description = "Pattern for file to generate languages into", Arity = ArgumentArity.ZeroOrOne
        };
        _rootCommand.Options.Add(_pathPattern);

        _lionWebVersion = new("--lionWebVersion")
        {
            Description = "LionWeb version", DefaultValueFactory = (_ => LionWebVersions.Current.VersionString)
        };
        _lionWebVersion.Validators.Add(r =>
        {
            switch (r.Tokens.Count)
            {
                case 0:
                    return;
                case > 1:
                    r.AddError($"invalid LionWebVersion: {r.Tokens}");
                    return;
                default:

                    var value = r.Tokens.First().Value;
                    if (LionWebVersions.AllPureVersions.All(v => v.VersionString != value))
                        r.AddError($"invalid LionWebVersion: {value}");
                    return;
            }
        });
        _rootCommand.Options.Add(_lionWebVersion);

        _writableIface =
            new("--writableInterfaces") { Arity = ArgumentArity.ZeroOrOne, DefaultValueFactory = _ => null };
        _rootCommand.Options.Add(_writableIface);

        _languageFile = new("languageFile")
        {
            Description = "LionWeb JSON file(s) containing languages to generate", Arity = ArgumentArity.ZeroOrMore
        };
        _languageFile.AcceptLegalFilePathsOnly();
        _languageFile.AcceptExistingOnly();
        _rootCommand.Arguments.Add(_languageFile);
    }

    public int ParseCommandLine()
    {
        var result = CheckParseErrors();

        if (result != 0)
            return result;

        result = ReadConfigFile();
        OverrideConfigs();

        return CreateCommandLineConfigs() ? 0 : result;
    }

    private int CheckParseErrors()
    {
        if (ParseResult.Errors.Count != 0)
        {
            foreach (var error in ParseResult.Errors)
            {
                LogError(error.ToString());
            }

            return -2;
        }

        return 0;
    }

    private int ReadConfigFile()
    {
        int result = -1;

        var configurationFile = ParseResult.GetValue(_config);
        if (configurationFile is not null)
        {
            var utf8JsonStream = configurationFile.OpenRead();
            var deserialized = JsonSerializer.Deserialize<Configuration[]>(utf8JsonStream);
            if (deserialized is not null)
            {
                var baseDir = configurationFile.Directory?.ToString() ?? "";
                foreach (var conf in deserialized)
                {
                    var languageFile = new FileInfo(Path.Combine(baseDir, conf.LanguageFile.ToString()));

                    var outputDir = conf is { OutputDir: null }
                        ? null
                        : new DirectoryInfo(Path.Combine(baseDir, conf.OutputDir.ToString()));

                    var outputFile = conf is { OutputFile: null }
                        ? null
                        : new FileInfo(Path.Combine(baseDir, conf.OutputFile.ToString()));

                    Configurations.Add(conf with
                    {
                        LanguageFile = languageFile, OutputDir = outputDir, OutputFile = outputFile
                    });
                }

                result = 0;
            }
        }

        return result;
    }

    private void OverrideConfigs()
    {
        foreach (var configuration in Configurations)
        {
            if (LionWebVersion is not null)
                configuration.LionWebVersion = LionWebVersion;

            if (OutputDir is not null && configuration.OutputFile is null)
                configuration.OutputDir = OutputDir;

            if (OutputFile is not null && configuration.OutputDir is null)
                configuration.OutputFile = OutputFile;

            if (Namespace is not null && !configuration.NamespacePattern.IsSet())
                configuration.Namespace = Namespace;

            if (NamespacePattern.IsSet() && configuration.Namespace is null)
                configuration.NamespacePattern = NamespacePattern;

            if (PathPattern.IsSet())
                configuration.PathPattern = (PathPattern)PathPattern!;

            if (GeneratorConfig is not null)
                configuration.GeneratorConfig = GeneratorConfig;
        }
    }

    private bool CreateCommandLineConfigs()
    {
        if (LanguageFiles is null)
            return false;

        Configurations.AddRange(LanguageFiles.Select(f =>
        {
            var configuration = new Configuration
            {
                LanguageFile = f,
                LionWebVersion = LionWebVersion,
                OutputDir = OutputDir,
                OutputFile = OutputFile,
                GeneratorConfig = GeneratorConfig
            };

            if (Namespace is not null)
                configuration.Namespace = Namespace;

            if (NamespacePattern.IsSet())
                configuration.NamespacePattern = NamespacePattern;

            if (PathPattern.IsSet())
                configuration.PathPattern = (PathPattern)PathPattern!;

            return configuration;
        }));

        return true;
    }

    private FileInfo[]? LanguageFiles => ParseResult.GetValue(_languageFile);
    private string? LionWebVersion => ParseResult.GetValue(_lionWebVersion);
    private DirectoryInfo? OutputDir => ParseResult.GetValue(_outputDir);
    private FileInfo? OutputFile => ParseResult.GetValue(_outputFile);
    private string? Namespace => ParseResult.GetValue(_ns);
    private NamespacePattern? NamespacePattern => ParseResult.GetValue(_namespacePattern);
    private PathPattern? PathPattern => ParseResult.GetValue(_pathPattern);

    private bool? WritableInterfaces
    {
        get
        {
            var optionResult = ParseResult.RootCommandResult.GetResult(_writableIface);
            var r = ParseResult.GetValue(_writableIface);
            return r;
        }
    }

    private GeneratorConfig GeneratorConfig
    {
        get
        {
            GeneratorConfig cfg = new();
            if (WritableInterfaces is { } b)
                cfg = cfg with { WritableInterfaces = b };
            return cfg;
        }
    }
    
    private void LogError(string message) => _errorLogger(message);
}