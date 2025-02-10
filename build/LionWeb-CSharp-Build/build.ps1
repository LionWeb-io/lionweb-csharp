$LocalChunks = "chunks/localDefs";
if (-Not (Test-Path $LocalChunks)) {
    New-Item -ItemType Directory -Path $LocalChunks
}

$TestProject = "../../test/LionWeb.Core.Test";
$GeneratedPath = $TestProject + "/Languages/Generated";
Remove-Item $GeneratedPath -Recurse
New-Item -ItemType Directory -Path $GeneratedPath
New-Item -ItemType Directory -Path $GeneratedPath/V2023_1
New-Item -ItemType Directory -Path $GeneratedPath/V2024_1

dotnet run Generate.cs --no-dependencies --no-restore

