namespace LionWeb.Generator.Test;

using Core;
using Core.Test.Languages.Generated.V2023_1.MultiInheritLang;
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
        
        Assert.HasCount(3, cSharpSyntaxNodes.OfType<ClassSyntaxNode>());
        Assert.IsNotNull(cSharpSyntaxNodes.First(node => node.Classifier.Name == nameof(testLanguage.LinkTestConcept)));
        Assert.IsNotNull(cSharpSyntaxNodes.First(node => node.Classifier.Name == nameof(testLanguage.DataTypeTestConcept)));
        Assert.IsNotNull(cSharpSyntaxNodes.First(node => node.Classifier.Name == nameof(testLanguage.TestAnnotation)));
    }
    
    [TestMethod]
    public void CSharpSyntaxNodes_of_interface()
    { 
        LionWebVersions lionWebVersion = LionWebVersions.v2023_1;
        var multiInheritLangLanguage = new MultiInheritLangLanguage("multiInheritLangLanguage");
        
        var generator = new GeneratorFacade
        {
            Names = new Names(multiInheritLangLanguage, "MultiInheritLangLanguage"),
            LionWebVersion = lionWebVersion
            
        };

        generator.Generate();
        
        var cSharpSyntaxNodes = generator.CSharpSyntaxNodes;
        
        Assert.HasCount(1, cSharpSyntaxNodes.OfType<InterfaceSyntaxNode>());
        Assert.IsNotNull(cSharpSyntaxNodes.Select(node => node.Classifier.Name == nameof(multiInheritLangLanguage.BaseIface)).First());
        Assert.IsNotNull(cSharpSyntaxNodes.OfType<InterfaceSyntaxNode>().First().InterfaceDeclarationSyntax);
    }
}