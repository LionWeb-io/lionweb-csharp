namespace LionWeb.Generator.Test;

using Core;
using Core.Test.Languages.Generated.V2023_1.MultiInheritLang;
using Core.Test.Languages.Generated.V2023_1.TestLanguage;
using GeneratorExtensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;

[TestClass]
public class ClassifierToSyntaxCorrelationTests
{
    [TestMethod]
    public void concept_and_annotation_as_classifiers()
    { 
        LionWebVersions lionWebVersion = LionWebVersions.v2023_1;
        var testLanguage = new TestLanguageLanguage("testLanguage");
        
        var generator = new GeneratorFacade
        {
            Names = new Names(testLanguage, "TestLanguage"),
            LionWebVersion = lionWebVersion
        };

        generator.Generate();
        var correlationData = generator.ClassifierToSyntaxCorrelationData;

        Assert.HasCount(3, correlationData.OfType<ClassSyntax>());

        var linkTestConceptSyntax = correlationData.First(node => node.Classifier.Name == nameof(testLanguage.LinkTestConcept));
        Assert.IsInstanceOfType<ClassSyntax>(linkTestConceptSyntax);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>((linkTestConceptSyntax as ClassSyntax)?.ClassDeclarationSyntax);
        Assert.AreEqual(nameof(testLanguage.LinkTestConcept), (linkTestConceptSyntax as ClassSyntax)?.ClassDeclarationSyntax.Identifier.ToString());

        var dataTypeTestConceptSyntax = correlationData.First(node => node.Classifier.Name == nameof(testLanguage.DataTypeTestConcept));
        Assert.IsInstanceOfType<ClassSyntax>(dataTypeTestConceptSyntax);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>((dataTypeTestConceptSyntax as ClassSyntax)?.ClassDeclarationSyntax);
        Assert.AreEqual(nameof(testLanguage.DataTypeTestConcept), (dataTypeTestConceptSyntax as ClassSyntax)?.ClassDeclarationSyntax.Identifier.ToString());

        var testAnnotationSyntax = correlationData.First(node => node.Classifier.Name == nameof(testLanguage.TestAnnotation));
        Assert.IsInstanceOfType<ClassSyntax>(testAnnotationSyntax);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>((testAnnotationSyntax as ClassSyntax)?.ClassDeclarationSyntax);
        Assert.AreEqual(nameof(testLanguage.TestAnnotation), (testAnnotationSyntax as ClassSyntax)?.ClassDeclarationSyntax.Identifier.ToString());
    }
    
    [TestMethod]
    public void interface_as_classifier()
    { 
        LionWebVersions lionWebVersion = LionWebVersions.v2023_1;
        var multiInheritLangLanguage = new MultiInheritLangLanguage("multiInheritLangLanguage");
        
        var generator = new GeneratorFacade
        {
            Names = new Names(multiInheritLangLanguage, "MultiInheritLangLanguage"),
            LionWebVersion = lionWebVersion
        };

        generator.Generate();
        var correlationData = generator.ClassifierToSyntaxCorrelationData;
        
        Assert.HasCount(1, correlationData.OfType<InterfaceSyntax>());
        Assert.IsInstanceOfType<InterfaceSyntax>(correlationData.First(node =>
            node.Classifier.Name == nameof(multiInheritLangLanguage.BaseIface)) as InterfaceSyntax);
        Assert.IsInstanceOfType<InterfaceDeclarationSyntax>(correlationData.OfType<InterfaceSyntax>().First().InterfaceDeclarationSyntax);
        Assert.AreEqual(nameof(multiInheritLangLanguage.BaseIface), correlationData.OfType<InterfaceSyntax>().First().InterfaceDeclarationSyntax.Identifier.ToString());
    }
}