cd build/LionWeb-CSharp-Build
./build.ps1
cd ../..

dotnet restore
dotnet build
dotnet test --no-restore
