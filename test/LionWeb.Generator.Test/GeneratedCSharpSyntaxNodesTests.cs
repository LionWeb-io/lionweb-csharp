namespace LionWeb.Generator.Test;

using Core;
using Core.Test.Languages.Generated.V2023_1.MultiInheritLang;
using Core.Test.Languages.Generated.V2023_1.TestLanguage;
using GeneratorExtensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        var cSharpSyntaxNodes = generator.CSharpSyntaxData;

        Assert.HasCount(3, cSharpSyntaxNodes.OfType<ClassSyntax>());

        var linkTestConceptSyntaxNode = cSharpSyntaxNodes.First(node => node.Classifier.Name == nameof(testLanguage.LinkTestConcept));
        Assert.IsInstanceOfType<ClassSyntax>(linkTestConceptSyntaxNode);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>((linkTestConceptSyntaxNode as ClassSyntax)?.ClassDeclarationSyntax);

        var dataTypeTestConceptSyntaxNode = cSharpSyntaxNodes.First(node => node.Classifier.Name == nameof(testLanguage.DataTypeTestConcept));
        Assert.IsInstanceOfType<ClassSyntax>(dataTypeTestConceptSyntaxNode);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>((dataTypeTestConceptSyntaxNode as ClassSyntax)?.ClassDeclarationSyntax);

        var testAnnotationSyntaxNode = cSharpSyntaxNodes.First(node => node.Classifier.Name == nameof(testLanguage.TestAnnotation));
        Assert.IsInstanceOfType<ClassSyntax>(testAnnotationSyntaxNode);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>((testAnnotationSyntaxNode as ClassSyntax)?.ClassDeclarationSyntax);
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
        var cSharpSyntaxNodes = generator.CSharpSyntaxData;
        
        Assert.HasCount(1, cSharpSyntaxNodes.OfType<InterfaceSyntax>());
        Assert.IsInstanceOfType<InterfaceSyntax>(cSharpSyntaxNodes.First(node =>
            node.Classifier.Name == nameof(multiInheritLangLanguage.BaseIface)) as InterfaceSyntax);
        Assert.IsInstanceOfType<InterfaceDeclarationSyntax>(cSharpSyntaxNodes.OfType<InterfaceSyntax>().First().InterfaceDeclarationSyntax);
    }
}