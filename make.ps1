# generate sources:
cd build/LionWeb-CSharp-Build
./build.ps1
cd ../..

# download the delta protocol JSON Schema:
Invoke-WebRequest "https://raw.githubusercontent.com/LionWeb-io/specification/refs/heads/main/delta/delta.schema.json" -OutFile test/LionWeb.Protocol.Delta.Test/resources/delta.schema.json

dotnet restore
dotnet build
dotnet test --no-restore
