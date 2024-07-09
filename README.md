# LionWeb-C#

(Or, really: `LionWeb-CSharp`.)

This repository contains an implementation of (select parts of) the [LionWeb](https://lionweb.io/) [specification](https://github.com/LionWeb-io/specification).


##  Organization

.NET projects:

* [`src/LionWeb-CSharp`]: implementation of LionWeb for/in C#
* [`src/LionWeb-CSharp-Generator`]: implementation of a C# code generator for LionWeb
* [`build/LionWeb-CSharp-build`]: a .NET console application that generates various source files in the `-Test` project
* [`test/LionWeb-CSharp-Test`]: unit tests for `LionWeb-CSharp`
* [`test/LionWeb-CSharp-Generator-Test`]: unit tests for `LionWeb-CSharp-Generator`

See these projects' respective `README`s for more information.


## CI

The CI is implemented through a GitHub Action, configured through the file [`build.yaml` workflow YAML file](./.github/workflows/build.yaml).
It's triggered:

* On every push to branch for which a Pull Request exists.
* When a commit (on any branch) is tagged.


## License

The "Apache-2.0" open-source license applies to the work in this repository.

