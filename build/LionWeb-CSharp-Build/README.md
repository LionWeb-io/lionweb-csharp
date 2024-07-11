# README

This .NET project contains code that generates code into the `LionWeb-CSharp-Test` .NET project.
Run the `build.ps1` PowerShell script to generate all code in the [`generated` folder](../../test/LionWeb-CSharp-Test/languages/generated).
It also generates a couple of language definitions in the LionWeb serialization chunk JSON format.

This essentially runs the [`Build.cs` (top-level style main program) entrypoint](./Generate.cs), equivalent to:

```shell
dotnet run Generate.cs
```

