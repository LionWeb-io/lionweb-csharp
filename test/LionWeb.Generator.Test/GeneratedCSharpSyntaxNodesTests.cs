namespace LionWeb.Generator.Test;

using Core;
using Core.M2;
using Core.M3;
using Core.Serialization;
using Io.Lionweb.Mps.Specific;
using Names;

[TestClass]
public class GeneratedCSharpSyntaxNodesTests
{
    [TestMethod]
    public void Test()
    { 
        LionWebVersions lionWebVersion = LionWebVersions.v2023_1;
        var specificLanguage = ISpecificLanguage.Get(lionWebVersion);
        var testLanguage = DeserializeExternalLanguage(lionWebVersion, "testLanguage", specificLanguage)[0];
        
        var generator = new GeneratorFacade
        {
            Names = new Names(testLanguage, "TestLanguage"),
            LionWebVersion = lionWebVersion
            
        };

        var compilationUnit = generator.Generate();
        var languageConstructSyntaxes = generator.GeneratedCSharpSyntaxNodes;
        
        Assert.IsEmpty(languageConstructSyntaxes);
    }

    DynamicLanguage[] DeserializeExternalLanguage(LionWebVersions lionWebVersion, string name,
        params Language[] dependentLanguages)
    {
        SerializationChunk serializationChunk =
            JsonUtils.ReadJsonFromString<SerializationChunk>(
                File.ReadAllText($"chunks/externalDefs/{lionWebVersion.VersionString}/{name}.json"));
        return new LanguageDeserializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build()
            .Deserialize(serializationChunk, dependentLanguages).ToArray();
    }
    
}