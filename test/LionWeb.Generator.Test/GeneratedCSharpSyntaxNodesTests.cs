namespace LionWeb.Generator.Test;

using Core;
using Core.Test.Languages.Generated.V2023_1.TestLanguage;
using Names;

[TestClass]
public class GeneratedCSharpSyntaxNodesTests
{
    [TestMethod]
    public void CSharpSyntaxNodesFromLanguage()
    { 
        LionWebVersions lionWebVersion = LionWebVersions.v2023_1;
        var testLanguage = new TestLanguageLanguage("testLanguage");
        
        var generator = new GeneratorFacade
        {
            Names = new Names(testLanguage, "TestLanguage"),
            LionWebVersion = lionWebVersion
            
        };

        generator.Generate();
        var generatedCSharpSyntaxNodes = generator.CSharpSyntaxNodes;
        
        Assert.HasCount(3, generatedCSharpSyntaxNodes);
        
    }
    
}