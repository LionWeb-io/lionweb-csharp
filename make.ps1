cd build/LionWeb-CSharp-build
pwsh build.ps1
cd ../..

dotnet restore
dotnet build
dotnet test --no-restore
