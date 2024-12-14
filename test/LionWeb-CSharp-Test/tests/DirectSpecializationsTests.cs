// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb_CSharp_Test.tests;

using Examples.Shapes.Dynamic;
using languages;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;

[TestClass]
public class DirectSpecializationsTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void LeafInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI4").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI1").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidI0") }, actual.ToList());
    }

    [TestMethod]
    public void MultipleInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI2").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidI1"), lang.ClassifierByKey("key-ValidI3") },
            actual.ToList());
    }

    [TestMethod]
    public void DirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-IA").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-IB"), }, actual.ToList());
    }

    [TestMethod]
    public void InterfaceWithImplementations()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI3").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                lang.ClassifierByKey("key-ValidI0"),
                lang.ClassifierByKey("key-ValidC1"),
                lang.ClassifierByKey("key-ValidA1")
            }, actual.ToList());
    }

    [TestMethod]
    public void LeafConcept()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC0").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleConcept()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC1").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidC0") }, actual.ToList());
    }

    [TestMethod]
    public void ConceptLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC2").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidC1") }, actual.ToList());
    }

    [TestMethod]
    public void DirectConceptCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-CA").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-CB"), }, actual.ToList());
    }

    [TestMethod]
    public void LeafAnnotation()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA0").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleAnnotation()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA1").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidA0") }, actual.ToList());
    }

    [TestMethod]
    public void AnnotationLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA2").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidA1") }, actual.ToList());
    }

    [TestMethod]
    public void DirectAnnotationCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-AA").DirectSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-AB"), }, actual.ToList());
    }

    [TestMethod]
    public void CrossLanguage()
    {
        Language lang = ShapesDynamic.Language;

        var actual = _builtIns.INamed.DirectSpecializations([lang, InvalidLanguage.Language]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-Shape"), lang.ClassifierByKey("key-Line"), }, actual.ToList());
    }
}