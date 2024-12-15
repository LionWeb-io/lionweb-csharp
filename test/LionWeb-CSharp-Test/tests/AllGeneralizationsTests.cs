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
public class AllGeneralizationsTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void RootInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI4").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI1").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidI2") }, actual.ToList());
    }

    [TestMethod]
    public void MultipleInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI7").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidI5"), lang.ClassifierByKey("key-ValidI6") },
            actual.ToList());
    }

    [TestMethod]
    public void InterfaceLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI8").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                lang.ClassifierByKey("key-ValidI7"),
                lang.ClassifierByKey("key-ValidI6"),
                lang.ClassifierByKey("key-ValidI5"),
            },
            actual.ToList());
    }


    [TestMethod]
    public void DirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-IA").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-IB"), }, actual.ToList());
    }

    [TestMethod]
    public void IndirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ID").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-IC"), lang.ClassifierByKey("key-IE"), }, actual.ToList());
    }

    [TestMethod]
    public void SecondaryDirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-IF").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-IG"), lang.ClassifierByKey("key-IH"), }, actual.ToList());
    }

    [TestMethod]
    public void DiamondInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-IJ").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                lang.ClassifierByKey("key-II"), lang.ClassifierByKey("key-IK"), lang.ClassifierByKey("key-IL")
            }, actual.ToList());
    }

    [TestMethod]
    public void ValidInterfaceDiamond()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI0").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                lang.ClassifierByKey("key-ValidI1"),
                lang.ClassifierByKey("key-ValidI2"),
                lang.ClassifierByKey("key-ValidI3")
            }, actual.ToList());
    }

    [TestMethod]
    public void RootConcept()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC3").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void DirectConceptCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-CA").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-CB"), }, actual.ToList());
    }

    [TestMethod]
    public void IndirectConceptCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-CD").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-CC"), lang.ClassifierByKey("key-CE"), }, actual.ToList());
    }

    [TestMethod]
    public void ValidConceptLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC0").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier>
        {
            lang.ClassifierByKey("key-ValidC1"),
            lang.ClassifierByKey("key-ValidC2"),
            lang.ClassifierByKey("key-ValidI0"),
            lang.ClassifierByKey("key-ValidI1"),
            lang.ClassifierByKey("key-ValidI2"),
            lang.ClassifierByKey("key-ValidI3"),
        }, actual.ToList());
    }

    [TestMethod]
    public void RootAnnotation()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA3").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void DirectAnnotationCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-AA").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-AB"), }, actual.ToList());
    }

    [TestMethod]
    public void IndirectAnnotationCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-AD").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-AC"), lang.ClassifierByKey("key-AE"), }, actual.ToList());
    }

    [TestMethod]
    public void ValidAnnotationLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA0").AllGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier>
        {
            lang.ClassifierByKey("key-ValidA1"),
            lang.ClassifierByKey("key-ValidA2"),
            lang.ClassifierByKey("key-ValidI0"),
            lang.ClassifierByKey("key-ValidI1"),
            lang.ClassifierByKey("key-ValidI2"),
            lang.ClassifierByKey("key-ValidI3"),
        }, actual.ToList());
    }

    [TestMethod]
    public void CrossLanguage()
    {
        Language lang = ShapesDynamic.Language;

        var actual = lang.ClassifierByKey("key-Circle").AllGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                _builtIns.INamed,
                lang.ClassifierByKey("key-Shape"),
                lang.ClassifierByKey("key-IShape")
            },
            actual.ToList());
    }
}