cd build/LionWeb-CSharp-Build
pwsh build.ps1
cd ../..

dotnet restore
dotnet build
dotnet test --no-restore
