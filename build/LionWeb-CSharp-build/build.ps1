$LocalChunks = "chunks/localDefs";
if (-Not (Test-Path $LocalChunks)) {
    New-Item -ItemType Directory -Path $LocalChunks
}

$TestProject = "../../test/LionWeb-CSharp-Test";
$GeneratedPath = $TestProject + "/languages/structure";
Remove-Item $GeneratedPath -Recurse
New-Item -ItemType Directory -Path $GeneratedPath

dotnet run Build.cs --no-dependencies --no-restore #--watch

$ExtDefs = "chunks/externalDefs";
$LocalDefs = "chunks/localDefs";
$TestProjectDefs = $TestProject + "/languages/defChunks";

cp $LocalDefs/shapesLanguage.json $TestProjectDefs/shapes.json
cp $ExtDefs/with-enum.json $TestProjectDefs/

$TestProjectFile = $TestProject + "/LionWeb-CSharp-Test.csproj";
dotnet build $TestProjectFile
dotnet test --no-restore $TestProjectFile
