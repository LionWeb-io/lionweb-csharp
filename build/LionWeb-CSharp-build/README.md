# README

This .NET project contains code that generates code into the `LionWeb-CSharp-Test` .NET project.
Execute the following on the command line (PowerShell):

```powershell
build.ps1
```

or (regular POSIX shell):

```shell
source build.sh
```

This essentially runs the [`Build.cs` (top-level style main program) entrypoint](./Build.cs), equivalent to:

```shell
dotnet run Build.cs
```

Look at the `// -> ` comments in that source file to see what it does exactly.

Adding the option `--watch` to the command line arguments of `Build.cs` runs the C# types generator step in _watch mode_.
That means that on any change to Handlebars template files in `LionWeb-CSharp/generator/templates` (including creating a new one), those templates files are reloaded and the generator is ran again.
That allows to quickly, conveniently, and comfortably change the generator templates.

