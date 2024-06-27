clear

mkdir -p chunks/localDefs

TEST_PROJECT=../../test/LionWeb-CSharp-Test

GENERATED_PATH=$TEST_PROJECT/languages/structure
rm -rf $GENERATED_PATH
mkdir $GENERATED_PATH

dotnet run Build.cs --no-dependencies --no-restore #--watch

EXT_DEFS=chunks/externalDefs
LOCAL_DEFS=chunks/localDefs
TEST_PROJECT_DEFS=$TEST_PROJECT/languages/defChunks

cp $LOCAL_DEFS/shapesLanguage.json $TEST_PROJECT_DEFS/shapes.json
cp $EXT_DEFS/with-enum.json $TEST_PROJECT_DEFS/

TEST_PROJECT_FILE=$TEST_PROJECT/LionWeb-CSharp-Test.csproj
dotnet build --no-dependencies --no-restore $TEST_PROJECT_FILE
dotnet test --no-restore $TEST_PROJECT_FILE

