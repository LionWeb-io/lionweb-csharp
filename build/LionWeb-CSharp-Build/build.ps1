$LocalChunks = "chunks/localDefs";
if (-Not (Test-Path $LocalChunks)) {
    New-Item -ItemType Directory -Path $LocalChunks
}

$TestProject = "../../test/LionWeb-CSharp-Test";
$GeneratedPath = $TestProject + "/languages/generated";
Remove-Item $GeneratedPath -Recurse
New-Item -ItemType Directory -Path $GeneratedPath

dotnet run Generate.cs --no-dependencies --no-restore

$ExtDefs = "chunks/externalDefs";
$LocalDefs = "chunks/localDefs";
$TestProjectDefs = $TestProject + "/languages/defChunks";

Move-Item -Path $LocalDefs/shapes.json -Destination $TestProjectDefs/ -Force
Copy-Item -Path $ExtDefs/with-enum.json -Destination $TestProjectDefs/
