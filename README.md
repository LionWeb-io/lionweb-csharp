# LionWeb-C#

(Or, really: `LionWeb-CSharp`.)

This repository contains an implementation of (select parts of) the [LionWeb](https://lionweb.io/) [specification](https://github.com/LionWeb-io/specification).


##  Organization

.NET projects:

* [`src/LionWeb.Core`]: implementation of LionWeb for/in C#
* [`src/LionWeb.Generator`]: implementation of a C# code generator for LionWeb
* [`build/LionWeb-CSharp-Build`]: a .NET console application that generates various source files in the `-Test` project
* [`test/LionWeb.Core.Test`]: unit tests for `LionWeb.Core`

See these projects' respective `README`s for more information.

Documentation can be found in the [`docs` folder](docs).


## CI

The CI is implemented through a GitHub Action, configured through the file [`build.yaml` workflow YAML file](./.github/workflows/build.yaml).
It's triggered:

* On every event for a Pull Request, including opening one.
* When a commit (on any branch) is tagged.
    Provided that restoring, building, packaging, and running all tests are all successful,
    then NuGet packages (for) `LionWeb.Core` and `LionWeb.Generator` are published with the tag as version identification.


## Development

Run the `make.ps1` PowerShell script to generate all required source files, build all projects, and run all unit tests.


## License

The "Apache-2.0" open-source license applies to the work in this repository.

