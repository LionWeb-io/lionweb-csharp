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

mv $LocalDefs/shapes.json $TestProjectDefs/
cp $ExtDefs/with-enum.json $TestProjectDefs/
