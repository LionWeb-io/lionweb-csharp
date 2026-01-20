// Copyright 2026 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Generator.Test;

using Core;
using Core.M2;
using Core.M3;
using Core.Test.Languages.Generated.V2024_1.DeprecatedLang;
using Core.Test.Languages.Generated.V2024_1.NamedLangReadInterfaces;
using Core.Test.Languages.Generated.V2024_1.SDTLang;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using GeneratorExtensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;

[TestClass]
public class KeyedToLanguageConstructorCorrelationTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.v2024_1;
    
    [TestMethod]
    public void AnnotationTest()
    {
        var lang = TestLanguageLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.TestAnnotation)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_testAnnotation", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(AnnotationBase<>));
    }

    [TestMethod]
    public void ConceptTest()
    {
        var lang = TestLanguageLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.LinkTestConcept)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_linkTestConcept", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(ConceptBase<>));
    }

    [TestMethod]
    public void InterfaceTest()
    {
        var lang = NamedReadInterfacesLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.Iface)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_iface", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(InterfaceBase<>));
    }

    [TestMethod]
    public void EnumTest()
    {
        var lang = TestLanguageLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.TestEnumeration)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_testEnumeration", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(EnumerationBase<>));
    }

    [TestMethod]
    public void PrimitiveTest()
    {
        var lang = DeprecatedLanguage.Instance;

        var generator = new GeneratorFacade
        {
            Names = new Names(lang, "GenTest")
            {
                PrimitiveTypeMappings =
                {
                    {
                        lang.FindByKey<PrimitiveType>(
                            "MDkzNjAxODQtODU5OC00NGU3LTliZjUtZmIxY2U0NWE0ODBhLzc4MTUyNDM0Nzk0ODc5OTM0ODQ"),
                        typeof(decimal)
                    }
                }
                
            }, LionWebVersion = _lionWebVersion
        };

        CompilationUnitSyntax compilationUnit = generator.Generate();
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.DeprDatatype)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_deprDatatype", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(PrimitiveTypeBase<>));
    }

    [TestMethod]
    public void SdtTest()
    {
        var lang = SDTLangLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.A)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_a", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(StructuredDataTypeBase<>));
    }

    [TestMethod]
    public void PropertyTest()
    {
        var lang = TestLanguageLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.DataTypeTestConcept_stringValue_0_1)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_dataTypeTestConcept_stringValue_0_1", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(PropertyBase<>));
    }

    [TestMethod]
    public void ContainmentTest()
    {
        var lang = TestLanguageLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.LinkTestConcept_containment_0_1)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_linkTestConcept_containment_0_1", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(ContainmentBase<>));
    }

    [TestMethod]
    public void ReferenceTest()
    {
        var lang = TestLanguageLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.LinkTestConcept_reference_0_n)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_linkTestConcept_reference_0_n", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(ReferenceBase<>));
    }

    [TestMethod]
    public void EnumLiteralTest()
    {
        var lang = TestLanguageLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.TestEnumeration_literal1)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_testEnumeration_literal1", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(EnumerationLiteralBase<>));
    }

    [TestMethod]
    public void FieldTest()
    {
        var lang = SDTLangLanguage.Instance;

        GeneratorFacade generator = Generate(lang, out CompilationUnitSyntax compilationUnit);
        var correlationManager = generator.Correlator;
        
        var correlation = correlationManager
            .FindAll<KeyedToLanguageConstructorCorrelation>(lang.A_a2b)
            .Single();
        var currentAst = correlation.LookupIn(compilationUnit);
        Assert.IsTrue(compilationUnit.Contains(currentAst));
        Assert.AreEqual("_a_a2b", ((IdentifierNameSyntax)currentAst.Left).ToString());
        AssertInitializerAst(currentAst, typeof(FieldBase<>));
    }

    private GeneratorFacade Generate(Language language, out CompilationUnitSyntax compilationUnit)
    {
        var generator = new GeneratorFacade
        {
            Names = new Names(language, "GenTest"), LionWebVersion = _lionWebVersion
        };

        compilationUnit = generator.Generate();
        return generator;
    }

    private static void AssertInitializerAst(AssignmentExpressionSyntax assignment, Type baseType) =>
        Assert.IsTrue(assignment.Right switch
        {
            ImplicitObjectCreationExpressionSyntax
            {
                ArgumentList.Arguments:
                [
                    {
                        Expression: ParenthesizedLambdaExpressionSyntax
                        {
                            Body: ObjectCreationExpressionSyntax
                            {
                                Type: GenericNameSyntax
                                {
                                    Identifier: { Text: {} baseTypeName },
                                    TypeArgumentList.Arguments:
                                    [
                                        IdentifierNameSyntax
                                    ]
                                },
                            }
                        }
                    }
                ]
            } when baseTypeName == baseType.Name.Replace("`1", "") => true,
            _ => false
        }, assignment.Right.ToString());
}