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
            Names = new Names(testLanguage, "TestLanguage"), LionWebVersion = lionWebVersion
        };

        var compilationUnit = generator.Generate();
        var correlationManager = generator.Correlator;

        Assert.HasCount(3, correlationManager.Correlations.OfType<ClassifierToMainCorrelation>());

        var linkTestCorrelation = correlationManager.FindAll<ClassifierToMainCorrelation>(testLanguage.LinkTestConcept)
            .Single();
        var currentLinkTest = linkTestCorrelation.LookupIn(compilationUnit);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>(currentLinkTest);
        Assert.AreEqual(nameof(testLanguage.LinkTestConcept), currentLinkTest.Identifier.ToString());
        Assert.IsTrue(compilationUnit.Contains(currentLinkTest));

        var dataTypeTestCorrelation = correlationManager
            .FindAll<ClassifierToMainCorrelation>(testLanguage.DataTypeTestConcept).Single();
        var currentDataTypeTest = dataTypeTestCorrelation.LookupIn(compilationUnit);
        Assert.IsInstanceOfType<ClassifierToMainCorrelation>(dataTypeTestCorrelation);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>(currentDataTypeTest);
        Assert.AreEqual(nameof(testLanguage.DataTypeTestConcept), currentDataTypeTest.Identifier.ToString());

        var testAnnotationCorrelation =
            correlationManager.FindAll<ClassifierToMainCorrelation>(testLanguage.TestAnnotation).Single();
        var currentTestAnnotation = testAnnotationCorrelation.LookupIn(compilationUnit);
        Assert.IsInstanceOfType<ClassifierToMainCorrelation>(testAnnotationCorrelation);
        Assert.IsInstanceOfType<ClassDeclarationSyntax>(currentTestAnnotation);
        Assert.AreEqual(nameof(testLanguage.TestAnnotation), currentTestAnnotation.Identifier.ToString());
    }

    [TestMethod]
    public void interface_as_classifier()
    {
        LionWebVersions lionWebVersion = LionWebVersions.v2023_1;
        var multiInheritLangLanguage = new MultiInheritLangLanguage("multiInheritLangLanguage");

        var generator = new GeneratorFacade
        {
            Names = new Names(multiInheritLangLanguage, "MultiInheritLangLanguage"), LionWebVersion = lionWebVersion
        };

        var compilationUnit = generator.Generate();
        var correlationManager = generator.Correlator;

        Assert.HasCount(1, correlationManager.Correlations.OfType<InterfaceToMainCorrelation>());
        var correlation = correlationManager.FindAll<InterfaceToMainCorrelation>(multiInheritLangLanguage.BaseIface)
            .Single();
        var currentIface = correlation.LookupIn(compilationUnit);
        Assert.IsInstanceOfType<InterfaceToMainCorrelation>(correlation);
        Assert.IsInstanceOfType<InterfaceDeclarationSyntax>(currentIface);
        Assert.AreEqual(nameof(multiInheritLangLanguage.BaseIface), currentIface.Identifier.ToString());
    }
}