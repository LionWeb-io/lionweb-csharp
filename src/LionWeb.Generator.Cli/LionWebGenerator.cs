namespace LionWeb.Generator.Cli;

using Core.M2;
using Core.M3;
using Core.Serialization;
using Io.Lionweb.Mps.Specific;
using Names;

public class LionWebGenerator
{
    static int Main(string[] args) =>
        new LionWebGenerator().Exec(args);

    private readonly List<Language> _allLanguages = [];
    public List<Configuration> ValidConfigurations { get; } = [];
    public List<Configuration> Configurations { get; } = [];

    public int Exec(string[] args)
    {
        var parser = new Parser(args);
        var result = parser.ParseCommandLine();
        Configurations.AddRange(parser.Configurations);
        if (result != 0)
        {
            return result;
        }

        result = LoadLanguages();
        result += GenerateLanguages();

        return result;
    }

    private int LoadLanguages()
    {
        var result = 0;

        ValidConfigurations.AddRange(Configurations
            .Select(LoadConfiguration)
            .Where(c =>
            {
                if (c is not null)
                    return true;

                result = -1;
                return false;
            })!);

        if (ValidConfigurations.Count == 0)
            result = -2;

        return result;
    }

    private Configuration? LoadConfiguration(Configuration configuration)
    {
        Console.WriteLine(configuration);

        if (!ValidateConfiguration(configuration))
            return null;

        if (!LoadLanguage(configuration))
            return null;

        Console.WriteLine();

        return configuration;
    }

    private static bool ValidateConfiguration(Configuration configuration)
    {
        if (!configuration.Validate(out var messages))
        {
            foreach (var message in messages)
            {
                Console.Error.WriteLine(message);
            }

            Console.Error.WriteLine("Skipping invalid configuration");
            Console.WriteLine();
            return false;
        }

        return true;
    }

    private bool LoadLanguage(Configuration configuration)
    {
        try
        {
            using var reader = configuration.LanguageFile.OpenText();

            var lionWebVersion = configuration.LionWebVersionParsed;
            var builder = new LanguageDeserializerBuilder().WithLionWebVersion(lionWebVersion)
                .WithDependentLanguages([ISpecificLanguage.Get(lionWebVersion)]);

            var chunk = JsonUtils.ReadJsonFromString<SerializationChunk>(reader.ReadToEnd());
            configuration.Languages = builder.Build()
                .Deserialize(chunk, _allLanguages.Where(l => l.LionWebVersion == lionWebVersion))
                .Cast<Language>()
                .ToList();

            if (configuration is { OutputFile: not null, Languages.Count: > 1 })
            {
                Console.Error.WriteLine(
                    $"Single output file {configuration.OutputFile} set, but language file {configuration.LanguageFile} contains more than one language");
                return false;
            }

            _allLanguages.AddRange(configuration.Languages);

            return true;
        } catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return false;
        }
    }

    private int GenerateLanguages()
    {
        var result = 0;

        foreach (var configuration in ValidConfigurations)
        {
            foreach (var language in configuration.Languages)
            {
                try
                {
                    Names names = PrepareNames(configuration, language);

                    Generate(names, configuration, language);
                } catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    result = -1;
                }
            }
        }

        return result;
    }

    private Names PrepareNames(Configuration configuration, Language language)
    {
        var namespaceName = configuration.GetNamespace(language);
        var names = new Names(language, namespaceName);

        // register all other known languages
        foreach ((Language otherLanguage, var ns) in ValidConfigurations
                     .SelectMany(c => c.Languages.Select(l => (l, c.GetNamespace(l))))
                     .Except([(language, namespaceName)]))
        {
            names.AddNamespaceMapping(otherLanguage, ns);
        }

        return names;
    }

    private static void Generate(Names names, Configuration configuration, Language language)
    {
        var generator = new GeneratorFacade
        {
            Names = names,
            LionWebVersion = configuration.LionWebVersionParsed,
            Config = configuration.GeneratorConfig ?? new GeneratorConfig()
        };

        generator.Generate();
        Console.WriteLine($"generated code for: {language.Name}");

        var path = configuration.GetFile(language);
        var outputDir = path.Directory;
        if (outputDir is not null && !outputDir.Exists)
        {
            Console.WriteLine($"Creating output directory {outputDir}");
            outputDir.Create();
        }

        generator.Persist(path.FullName);
        Console.WriteLine($"persisted to: {path}");
    }
}