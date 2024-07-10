cd build/LionWeb-CSharp-build
pwsh build.ps1
cd ../..

cd test/LionWeb-CSharp-Generator-Test
dotnet run Generate.cs
cd ../..

dotnet restore
dotnet build
dotnet test --no-restore
