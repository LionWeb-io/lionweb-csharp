# README

This project contains a command-line interface for the [LionWeb C# generator](https://github.com/LionWeb-io/lionweb-csharp/tree/main/src/LionWeb.Generator).

We can configure the generator via command-line options, or a configuration file.
If we provide both, command-line options override the corresponding setting in the configuration file.

## Installation

```
dotnet tool install LionWeb-CSharp-Generator-Cli
```

## Usage

```
LionWebGenerator [<languageFile>...] [options]
```
### Arguments
### languageFile
LionWeb JSON file(s) containing languages to generate

### Options

#### config
```
--config <config>
```
Specifies the configuration file.

The configuration file is a JSON file containing an array of configuration objects.
The configuration objects can contain all arguments and options available at command-line (starting with uppercase letter).

All paths inside the configuration file are relative to the configuration file itself.
In the first example below, if the configuration file is at `my/dir/config.json`, we expect the language file at `my/dir/testLanguage.2023_1.json`, and generate to `my/dir/out/generated.cs`.

Example:

```json
[
  {
    "LanguageFile": "testLanguage.2023_1.json",
    "OutputFile": "out/generated.cs",
    "Namespace": "My.Name.Space",
    "LionWebVersion": "2023.1"
  },
  {
    "LanguageFile": "severalLanguages.json",
    "OutputDir": "out/2024",
    "Namespace": "My.Name.Space",
    "PathPattern": "NamespaceInFilename",
    "DotGSuffix": false,
    "LionWebVersion": "2024.1",
    "GeneratorConfig": {
      "WritableInterfaces": true
    }
  }
]
```

#### outputDir
```
--output, --outputDir <outputDir>
```

Specifies the directory to write the generated file(s) to.

Use in conjunction with [pathPattern](#pathpattern).
Cannot be used together with [outputFile](#outputfile).

#### outputFile
```
--outputFile <outputFile>
```

Specifies the file to generate the language into.

Cannot be used if
* we specify more than one [languageFile](#languagefile)
* any of the languageFiles contain more than one language
* together with [outputDir](#outputdir)
* together with [pathPattern](#pathpattern)
* together with [dotGSuffix](#dotgsuffix)

#### pathPattern
```
--pathPattern
    VerbatimName (default) |
    VerbatimKey |
    NamespaceAsFilename |
    NamespaceAsPath |
    NamespaceInFilename |
    NamespaceInPath
```

Specifies how to derive the generated file name from the generated language.

* `VerbatimName`:: Uses the language's name as file name.
* `VerbatimKey`:: Uses the language's key as file name.
* `NamespaceAsFilename`:: Use the namespace as filename, e.g. `My.Nice.LanguageName.cs`.
  Useful in conjunction with [namespacePattern](#namespacepattern).
* `NamespaceAsPath`:: Convert each namespace segment to a path segment, e.g. `My/Nice/LanguageName.cs`.
  Useful in conjunction with [namespacePattern](#namespacepattern).
* `NamespaceInFilename`:: Use the namespace as filename and attach the language name, e.g. `My.Predefined.NameSpace.LanguageName.cs`.
  Useful in conjunction with explicit [namespace](#namespace).
* `NamespaceInPath`:: Convert each namespace segment to a directory and attach the language name, e.g. `My/Predefined/NameSpace/LanguageName.cs`.
  Useful in conjunction with explicit [namespace](#namespace).

Use in conjunction with [outputDir](#outputdir).
Cannot be used together with [outputFile](#outputfile).

#### dotGSuffix
```
--dotGSuffix
    true (default) |
    false
```

Specifies whether output file should have `.g` suffix before `.cs`, defaults to `true`.

Examples:
* `true`:: `MyLang.g.cs`
* `false`:: `MyLang.cs`

Use in conjunction with [outputDir](#outputdir).
Cannot be used together with [outputFile](#outputfile).

#### namespace
```
--namespace <namespace>
```

Explicitly specify the Namespace to generate languages into.
Must be a valid C# namespace.

Useful in conjunction with [pathPattern](#pathpattern)s `NamespaceInFilename` and `NamespaceInPath`.

Cannot be used together with [namespacePattern](#namespacepattern).

#### namespacePattern
```
--namespacePattern
    DotSeparated |
    DotSeparatedFirstUppercase
```

Specifies how convert the generated language's name into the Namespace to generate the language into.

* `DotSeparated`:: Split the language's name into segments, separated by a dot (`.`), e.g. `My.nice.languageName` -> [`My`, `nice`, `languageName`].
* `DotSeparatedFirstUppercase`:: Split the language's name into segments, separated by a dot (`.`), and turn the first letter of each segment to upper case, e.g. `My.nice.languageName` -> [`My`, `Nice`, `LanguageName`].

Useful in conjunction with [pathPattern](#pathpattern)s `NamespaceAsFilename` and `NamespaceAsPath`.

Cannot be used together with [namespace](#namespace).

#### lionWebVersion
```
--lionWebVersion <lionWebVersion>
```

Specifies the LionWeb version to use in the generator, e.g. `2023.1` or `2024.1`.

Defaults to the current LionWeb version.

#### writableInterfaces
```
--writableInterfaces
    true (default) |
    false
```

Specifies whether generated interfaces should implement `IWritableNode`, defaults to `true`.
* `true`:: generated interfaces implement `IWritableNode`
* `false`:: generated interfaces implement `IReadableNode`
