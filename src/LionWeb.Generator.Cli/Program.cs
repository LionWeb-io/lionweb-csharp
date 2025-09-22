namespace LionWeb.Generator.Cli;

using Core;
using Core.M2;
using Core.M3;
using Core.Serialization;
using Names;
using System.CommandLine;
using System.Text.RegularExpressions;

class Program
{
    static int Main(string[] args) =>
        new Program().Exec(args);

    private readonly Option<FileInfo> _config;

    private readonly Argument<FileInfo[]> _languageFile;

    private readonly RootCommand _rootCommand;

    private readonly Option<DirectoryInfo> _outputDir;

    private readonly Option<string> _ns;

    private readonly Option<string> _lionWebVersion;

    private readonly Option<bool> _writableIface;

    public Program()
    {
        _rootCommand = new("Generates C# classes from LionWeb languages");

        _config = new("--config") { Description = "Configuration file", Arity = ArgumentArity.ZeroOrOne, };
        _config.AcceptLegalFilePathsOnly();
        _config.AcceptExistingOnly();
        _rootCommand.Options.Add(_config);

        _languageFile = new("language file")
        {
            Description = "LionWeb JSON file(s) containing languages to generate", Arity = ArgumentArity.ZeroOrMore
        };
        _languageFile.AcceptLegalFilePathsOnly();
        _languageFile.AcceptExistingOnly();
        _rootCommand.Arguments.Add(_languageFile);

        _outputDir = new("--output")
        {
            Description = "Directory to write generated files to", Arity = ArgumentArity.ZeroOrOne
        };
        _outputDir.AcceptLegalFilePathsOnly();
        _rootCommand.Options.Add(_outputDir);

        _ns = new("--namespace")
        {
            Description = "Namespace to generate languages into",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = r => ""
        };
        _ns.Validators.Add(s =>
            {
                var value = s.GetValue(_ns);
                if (value is not null && value.Length > 0 &&
                    !Regex.IsMatch(value, @"^(@?[a-z_A-Z]\w+(?:\.@?[a-z_A-Z]\w+)*)$"))
                    s.AddError($"Not a valid namespace: {value}");
            }
        );
        _rootCommand.Options.Add(_ns);

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
                    if (!LionWebVersions.AllPureVersions.Any(v => v.VersionString == value))
                        r.AddError($"invalid LionWebVersion: {value}");
                    return;
            }
        });
        _rootCommand.Options.Add(_lionWebVersion);

        _writableIface = new("--writableInterfaces") { DefaultValueFactory = r => false };
        _rootCommand.Options.Add(_writableIface);
    }

    private int Exec(string[] args)
    {
        var parseResult = ParseCommandLine(args, out List<Configuration> configurations);
        if (parseResult != 0)
        {
            return parseResult;
        }

        List<Language> languages = [];

        foreach (var configuration in configurations)
        {
            Console.WriteLine(configuration);
            
            using var reader = configuration.LanguageFile.OpenText();

            var builder = new LanguageDeserializerBuilder()
                .WithLionWebVersion(configuration.LionWebVersionParsed);

            var chunk = JsonUtils.ReadJsonFromString<SerializationChunk>(reader.ReadToEnd());
            configuration.Languages = builder.Build().Deserialize(chunk, languages).Cast<Language>().ToList();
            languages.AddRange(configuration.Languages);

            foreach (var language in configuration.Languages)
            {
                var generator = new GeneratorFacade
                {
                    Names = new Names(language, configuration.Namespc),
                    LionWebVersion = configuration.LionWebVersionParsed,
                    Config = configuration.GeneratorConfig
                };

                generator.Generate();
                Console.WriteLine($"generated code for: {language.Name}");

                var path = @$"{configuration.OutputDir}/{language.Name}.g.cs";
                configuration.OutputDir.Create();
                generator.Persist(path);
                Console.WriteLine($"persisted to: {path}");
            }
        }

        return 0;
    }

    private int ParseCommandLine(string[] args, out List<Configuration> configurations)
    {
        var parseResult = _rootCommand.Parse(args);

        if (parseResult.Errors.Count != 0)
        {
            foreach (var error in parseResult.Errors)
            {
                Console.Error.WriteLine(error);
            }

            configurations = [];
            return -1;
        }

        var languageFiles = parseResult.GetValue(_languageFile);
        var version = parseResult.GetValue(_lionWebVersion);
        var output = parseResult.GetValue(_outputDir);
        var namespc = parseResult.GetValue(_ns);
        var writableInterfaces = parseResult.GetValue(_writableIface);
        var cfg = new GeneratorConfig { WritableInterfaces = writableInterfaces };

        if (languageFiles is null || version is null || output is null || namespc is null)
        {
            configurations = [];
            return -1;
        }

        configurations = languageFiles.Select(f => new Configuration(f, version, output, namespc, cfg)).ToList();
        return 0;
    }
}

public record Configuration(
    FileInfo LanguageFile,
    string LionWebVersion,
    DirectoryInfo OutputDir,
    string Namespc,
    GeneratorConfig GeneratorConfig)
{
    public List<Language> Languages { get; set; }

    public LionWebVersions LionWebVersionParsed =>
        LionWebVersions.GetPureByVersionString(LionWebVersion);

    public override string ToString() =>
        $"""
         Language file: {LanguageFile}
         LionWeb version: {LionWebVersion}
         Output directory: {OutputDir}
         Namespace: {Namespc}
         Generator config: {GeneratorConfig}
         """;
}