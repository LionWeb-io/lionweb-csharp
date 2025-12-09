namespace LionWeb.Generator.Test;

using Core;
using Core.Test.Languages.Generated.V2023_1.TestLanguage;
using Names;

[TestClass]
public class GeneratedCSharpSyntaxNodesTests
{
    [TestMethod]
    public void CSharpSyntaxNodes_of_concept_and_annotation()
    { 
        LionWebVersions lionWebVersion = LionWebVersions.v2023_1;
        var testLanguage = new TestLanguageLanguage("testLanguage");
        
        var generator = new GeneratorFacade
        {
            Names = new Names(testLanguage, "TestLanguage"),
            LionWebVersion = lionWebVersion
            
        };

        generator.Generate();
        
        var cSharpSyntaxNodes = generator.CSharpSyntaxNodes;
        
        Assert.HasCount(3, cSharpSyntaxNodes);
        Assert.IsNotNull(cSharpSyntaxNodes.First(node => ((ClassSyntaxNode)node).Classifier.Name == nameof(LinkTestConcept)));
        Assert.IsNotNull(cSharpSyntaxNodes.First(node => ((ClassSyntaxNode)node).Classifier.Name == nameof(DataTypeTestConcept)));
        Assert.IsNotNull(cSharpSyntaxNodes.First(node => ((ClassSyntaxNode)node).Classifier.Name == nameof(TestAnnotation)));
    }
    
}