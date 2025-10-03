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
using Core.M3;
using Impl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public record Configuration
{
    private const char _namespaceSeparator = '.';

    [JsonConverter(typeof(FileInfoConverter))]
    public FileInfo? LanguageFile { get; init; }

    [JsonConverter(typeof(DirectoryInfoConverter))]
    public DirectoryInfo? OutputDir { get; set; }

    [JsonConverter(typeof(FileInfoConverter))]
    public FileInfo? OutputFile { get; set; }

    public NamespacePattern? NamespacePattern { get; set; }
    public string? Namespace { get; set; }

    public PathPattern? PathPattern { get; set; }
    public bool? DotGSuffix { get; set; }

    public string? LionWebVersion { get; set; }
    public GeneratorConfig? GeneratorConfig { get; set; }
    public List<Language> Languages { get; set; }

    public LionWebVersions LionWebVersionParsed =>
        LionWebVersions.GetPureByVersionString(LionWebVersion);

    public bool Validate(out List<string> messages)
    {
        var valid = true;
        messages = [];

        valid &= ValidateLanguageFile(messages);
        valid &= ValidateOutput(messages);
        valid &= ValidateNamespace(messages);
        valid &= ValidatePathPattern(messages);
        valid &= ValidateLionWebVersion(messages);
        valid &= ValidateGeneratorConfig(messages);

        return valid;
    }

    private bool ValidateLanguageFile(List<string> messages)
    {
        if (LanguageFile is not { Exists: true })
        {
            messages.Add($"{nameof(LanguageFile)} doesn't exist: {LanguageFile?.ToString() ?? "null"}");
            return false;
        }

        return true;
    }

    private bool ValidateOutput(List<string> messages)
    {
        if (OutputDir == null && OutputFile == null)
        {
            messages.Add($"Neither {nameof(OutputDir)} nor {nameof(OutputFile)} set");
            return false;
        }

        if (OutputDir != null && OutputFile != null)
        {
            messages.Add(
                $"Both {nameof(OutputDir)} ({OutputDir}) and {nameof(OutputFile)} ({OutputFile}) set");

            return false;
        }

        if (OutputDir is { Exists: false })
        {
            messages.Add($"{nameof(OutputDir)} doesn't exist: {OutputDir}");
        }
        
        if (OutputFile is { Directory.Exists: false })
        {
            messages.Add($"{nameof(OutputFile)}'s parent directory doesn't exist: {OutputFile}");
        }

        if (OutputFile is not null)
        {
            if (DotGSuffix is not null)
                messages.Add($"{nameof(DotGSuffix)} ignored because {nameof(OutputFile)} is set");

            if (PathPattern is not null)
                messages.Add($"{nameof(PathPattern)} ignored because {nameof(OutputFile)} is set");
        }

        return true;
    }

    private bool ValidateNamespace(List<string> messages)
    {
        bool valid = true;
        
        if (Namespace is { Length: > 0 })
        {
            if (!NamespaceUtil.NamespaceRegex().IsMatch(Namespace))
            {
                messages.Add($"Not a valid namespace: '{Namespace}'");
                valid = false;
            }
        }

        if (!NamespacePattern.IsSet() && Namespace == null)
        {
            messages.Add($"Neither {nameof(Namespace)} nor {nameof(NamespacePattern)} set");
            valid = false;
        } else if (NamespacePattern.IsSet() && Namespace != null)
        {
            messages.Add(
                $"Both {nameof(Namespace)} ({Namespace}) and {nameof(NamespacePattern)} ({NamespacePattern}) set");
            valid = false;
        }

        if (NamespacePattern.IsSet() && Enum.GetName((NamespacePattern)NamespacePattern) == null)
        {
            messages.Add($"Unknown value for {nameof(NamespacePattern)}: {NamespacePattern}");
            valid = false;
        }

        return valid;
    }

    private bool ValidatePathPattern(List<string> messages)
    {
        if (!PathPattern.IsSet())
        {
            messages.Add($"{nameof(PathPattern)} not set");
            return false;
        }

        if (Enum.GetName((PathPattern)PathPattern) == null)
        {
            messages.Add($"Unknown value for {nameof(PathPattern)}: {PathPattern}");
            return false;
        }

        return true;
    }

    private bool ValidateLionWebVersion(List<string> messages)
    {
        if (string.IsNullOrEmpty(LionWebVersion))
        {
            messages.Add($"{LionWebVersion} not set");
            return false;
        }
        
        try
        {
            var _ = LionWebVersionParsed;
        } catch (UnsupportedVersionException e)
        {
            messages.Add($"Unsupported {nameof(LionWebVersion)}: {LionWebVersion}");
            return false;
        }

        return true;
    }

    private bool ValidateGeneratorConfig(List<string> messages)
    {
        if (GeneratorConfig == null)
        {
            messages.Add($"{nameof(GeneratorConfig)} not set");
            return false;
        }

        return true;
    }

    public string GetNamespace(Language language)
    {
        if (Namespace != null)
            return Namespace;

        return NamespacePattern switch
        {
            { } p when !p.FirstToUpper() =>
                string.Join(_namespaceSeparator, language.Name.Split(_namespaceSeparator)),
            { } p when p.FirstToUpper() =>
                string.Join(_namespaceSeparator,
                    language.Name.Split(_namespaceSeparator).Select(s => s.ToFirstUpper())),
            _ => throw new UnknownEnumValueException<NamespacePattern?>(NamespacePattern)
        };
    }

    public FileInfo GetFile(Language language)
    {
        if (OutputFile != null)
            return OutputFile;

        var ns = GetNamespace(language);

        var fileInfo = PathPattern switch
        {
            Cli.PathPattern.VerbatimName => language.Name,
            Cli.PathPattern.VerbatimKey => language.Key,
            Cli.PathPattern.NamespaceInFilename => Split(_namespaceSeparator, ns) + _namespaceSeparator + language.Name,
            Cli.PathPattern.NamespaceAsFilename => Split(_namespaceSeparator, ns),
            Cli.PathPattern.NamespaceInPath => Split(Path.DirectorySeparatorChar, ns) + Path.DirectorySeparatorChar +
                                           language.Name,
            Cli.PathPattern.NamespaceAsPath => Split(Path.DirectorySeparatorChar, ns),
            _ => throw new UnknownEnumValueException<PathPattern?>(PathPattern)
        };

        var suffix = DotGSuffix ?? true ? ".g.cs" : ".cs";
        return new FileInfo(Path.Combine(OutputDir?.ToString() ?? "", fileInfo + suffix));
    }

    private static string Split(char separator, string languageNamespace) => string.Join(separator,
        languageNamespace.Split(_namespaceSeparator).Select(p =>
        {
            var stringBuilder = new StringBuilder(p);
            foreach (var invalidChar in Path.GetInvalidFileNameChars().Append('\\').Append('/'))
            {
                stringBuilder.Replace(invalidChar, '_');
            }

            return stringBuilder.ToString();
        }));

    /// <inheritdoc />
    public override string ToString()
    {
        var ns = (Namespace, NamespacePattern) switch
        {
            (null, { } p) => $"Namespace pattern: {p}",
            ({ } s, null) => $"Namespace: {s}",
            _ => $"!! Namespace: {Namespace}, Namespace pattern: {NamespacePattern}"
        };

        var output = (OutputDir, OutputFile) switch
        {
            (null, { } f) => $"Output file: {f}",
            ({ } d, null) => $"Output directory: {d}",
            _ => $"!! Output directory: {OutputDir}, Output file: {OutputFile}"
        };

        return $"""
                Language file: {LanguageFile}
                {output}
                LionWeb version: {LionWebVersion}
                {ns}
                Path pattern: {PathPattern}
                Insert .g file name suffix: {DotGSuffix}
                Generator config: {GeneratorConfig}
                """;
    }

    public virtual bool Equals(Configuration? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(LanguageFile.FullName, other.LanguageFile.FullName, StringComparison.InvariantCulture) &&
               string.Equals(OutputDir?.FullName, other.OutputDir?.FullName, StringComparison.InvariantCulture) &&
               string.Equals(OutputFile?.FullName, other.OutputFile?.FullName, StringComparison.InvariantCulture) &&
               NamespacePattern == other.NamespacePattern &&
               PathPattern == other.PathPattern &&
               string.Equals(Namespace, other.Namespace, StringComparison.InvariantCulture) &&
               string.Equals(LionWebVersion, other.LionWebVersion, StringComparison.InvariantCulture) &&
               Equals(GeneratorConfig, other.GeneratorConfig);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(LanguageFile);
        hashCode.Add(OutputDir);
        hashCode.Add(OutputFile);
        hashCode.Add(NamespacePattern);
        hashCode.Add(PathPattern);
        hashCode.Add(Namespace, StringComparer.InvariantCulture);
        hashCode.Add(LionWebVersion, StringComparer.InvariantCulture);
        hashCode.Add(GeneratorConfig);
        return hashCode.ToHashCode();
    }
}

internal class FileInfoConverter : JsonConverter<FileInfo>
{
    public override FileInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s is not null && s.Length > 0)
            return new(s);
        return null;
    }

    public override void Write(Utf8JsonWriter writer, FileInfo value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

internal class DirectoryInfoConverter : JsonConverter<DirectoryInfo>
{
    public override DirectoryInfo? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s is not null && s.Length > 0)
            return new(s);
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DirectoryInfo value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}