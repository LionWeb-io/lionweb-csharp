# README

This .NET project contains code that generates code into the `LionWeb-CSharp-Test` .NET project.
Execute the following on the command line (PowerShell):

```powershell
build.ps1
```

This essentially runs the [`Build.cs` (top-level style main program) entrypoint](./Generate.cs), equivalent to:

```shell
dotnet run Generate.cs
```

This generates all code in the [`generated` folder](../../test/LionWeb-CSharp-Test/languages/generated).
It also generates a couple of language definitions in the LionWeb serialization chunk JSON format.

