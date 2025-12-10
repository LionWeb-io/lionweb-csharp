# LionWeb-C#

(Or, really: `LionWeb-CSharp`.)

This repository contains an implementation of (select parts of) the [LionWeb](https://lionweb.io/) [specification](https://github.com/LionWeb-io/specification).


##  Organization

.NET projects:

* [`src/LionWeb.Core`]: implementation of LionWeb for/in C#
* [`src/LionWeb.Generator`]: implementation of a C# code generator for LionWeb
* [`src/LionWeb.Generator.Cli`]: command-line interface for `LionWeb.Generator`
* [`src/LionWeb.Generator.MpsSpecfic`]: M2 extensions used by `LionWeb.Generator`
* [`src/LionWeb.Protocol.Delta`]: implementation of LionWeb delta protocol
* [`build/LionWeb-CSharp-Build`]: a .NET console application that generates various source files in the `-Test` project
* [`test/LionWeb.Core.Benchmark`]: Benchmarks for different aspects of LionWeb C#
* [`test/LionWeb.Core.Test`]: unit tests for `LionWeb.Core`
* [`test/LionWeb.Core.Test.Languages`]: Generated languages to be used in tests.
* [`test/LionWeb.Generator.Cli.Test`]: unit tests for `LionWeb.Generator.Cli`
* [`test/LionWeb.Protocol.Delta.Test`]: unit tests for `LionWeb.Protocol.Delta`

See these projects' respective `README`s for more information.

Documentation can be found in the [`docs` folder](docs).


## CI

The CI is implemented through a GitHub Action, configured through the file [`build.yaml` workflow YAML file](./.github/workflows/build.yaml).
It's triggered:

* On every event for a Pull Request, including opening one.
* When a commit (on any branch) is tagged.
    Provided that restoring, building, packaging, and running all tests are all successful,
    then NuGet packages (for) `LionWeb-CSharp` and `LionWeb-CSharp-Generator` are published to [NuGet “Central”](https://www.nuget.org/) with the tag as version identification.

### NuGet API keys

Publishing to [NuGet “Central”](https://www.nuget.org/) requires an API key that's managed through that site.
To manage the API key, you need an account that's an administrator member of the [LionWeb organization](https://www.nuget.org/profiles/LionWeb) on that site.
After logging in, navigate to the [API keys page](https://www.nuget.org/account/apikeys).
At least one of the keys listed there should have the following characteristics:

* “Push” permission — either “new packages and package versions” or “only new package versions”, but preferably the former.
* Package owner: LionWeb
* Packages: `LionWeb-CSharp`, `LionWeb-CSharp-Generator`; or Glob pattern: `*`

**Note**! Whenever you have to (re-)generate an API key, make sure that the key is associated with all of the LionWeb-CSharp packages to publish.
If it's not, then you click the “Edit” link, and *then* re-generate the key.
(API keys are apparently associated with packages at key generation time.)
If that's not the case, you'll get the following slightly misleading error message during publishing:

    error: Response status code does not indicate success: 403 (The specified API key is invalid, has expired, or does not have permission to access the specified package.).

After generating the API key, you should copy it (to the clipboard) using the “Copy” link, and then configure it as the value of the `NUGET_APIKEY` secret within the GitHub settings for Actions.
You find [these settings here](https://github.com/LionWeb-io/lionweb-csharp/settings/secrets/actions).


## Development

Run the `make.ps1` PowerShell script to:

* generate all required source files,
* download the required JSON Schema for the delta protocol into the `LionWeb.Protocol.Delta.Test` project,
* build all projects,
* and run all unit tests.


## License

The "Apache-2.0" open-source license applies to the work in this repository.

