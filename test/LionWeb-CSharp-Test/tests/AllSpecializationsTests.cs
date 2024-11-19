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

// ReSharper disable SuggestVarOrType_Elsewhere

namespace LionWeb_CSharp_Test.tests;

using Examples.Shapes.Dynamic;
using languages;
using LionWeb.Core.M2;
using LionWeb.Core.M3;

[TestClass]
public class AllSpecializationsTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void Interface()
    {
        Language lang = ShapesDynamic.Language;

        var actual = lang.ClassifierByKey("key-IShape").AllSpecializations([ShapesDynamic.Language]);

        CollectionAssert.AreEquivalent(new List<Classifier>
        {
            lang.ClassifierByKey("key-Circle"),
            lang.ClassifierByKey("key-Line"),
            lang.ClassifierByKey("key-OffsetDuplicate"),
            lang.ClassifierByKey("key-CompositeShape"),
            lang.ClassifierByKey("key-Shape"),
        }, actual.ToList());
    }

    [TestMethod]
    public void Concept()
    {
        Language lang = ShapesDynamic.Language;

        var actual = lang.ClassifierByKey("key-Shape").AllSpecializations([ShapesDynamic.Language]);

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                lang.ClassifierByKey("key-Circle"),
                lang.ClassifierByKey("key-Line"),
                lang.ClassifierByKey("key-OffsetDuplicate"),
                lang.ClassifierByKey("key-CompositeShape")
            }, actual.ToList());
    }

    [TestMethod]
    public void Annotation()
    {
        Language lang = ShapesDynamic.Language;

        var actual = lang.ClassifierByKey("key-Documentation").AllSpecializations([ShapesDynamic.Language]);

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void DirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-IA").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-IB"), }, actual.ToList());
    }

    [TestMethod]
    public void IndirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ID").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-IC"), lang.ClassifierByKey("key-IE"), }, actual.ToList());
    }

    [TestMethod]
    public void SecondaryDirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-IG").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-IF"), lang.ClassifierByKey("key-IH"), }, actual.ToList());
    }

    [TestMethod]
    public void DiamondInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-IJ").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier>
        {
            lang.ClassifierByKey("key-II"),
            lang.ClassifierByKey("key-IK"),
            lang.ClassifierByKey("key-IL"),
            lang.ClassifierByKey("key-CF"),
            lang.ClassifierByKey("key-AF"),
        }, actual.ToList());
    }

    [TestMethod]
    public void ValidInterfaceDiamond()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI2").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier>
        {
            lang.ClassifierByKey("key-ValidI0"),
            lang.ClassifierByKey("key-ValidI1"),
            lang.ClassifierByKey("key-ValidI3"),
            lang.ClassifierByKey("key-ValidC0"),
            lang.ClassifierByKey("key-ValidC1"),
            lang.ClassifierByKey("key-ValidC2"),
            lang.ClassifierByKey("key-ValidA0"),
            lang.ClassifierByKey("key-ValidA1"),
            lang.ClassifierByKey("key-ValidA2"),
        }, actual.ToList());
    }

    [TestMethod]
    public void DirectConceptCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-CA").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-CB"), }, actual.ToList());
    }

    [TestMethod]
    public void IndirectConceptCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-CD").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-CC"), lang.ClassifierByKey("key-CE"), }, actual.ToList());
    }

    [TestMethod]
    public void ValidConceptLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC2").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidC0"), lang.ClassifierByKey("key-ValidC1"), },
            actual.ToList());
    }

    [TestMethod]
    public void DirectAnnotationCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-AA").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-AB"), }, actual.ToList());
    }

    [TestMethod]
    public void IndirectAnnotationCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-AD").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-AC"), lang.ClassifierByKey("key-AE"), }, actual.ToList());
    }

    [TestMethod]
    public void ValidAnnotationLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA2").AllSpecializations([lang]);

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidA0"), lang.ClassifierByKey("key-ValidA1"), },
            actual.ToList());
    }

    [TestMethod]
    public void CrossLanguage()
    {
        Language lang = ShapesDynamic.Language;

        var actual = _builtIns.INamed.AllSpecializations([lang, InvalidLanguage.Language]);

        CollectionAssert.AreEquivalent(new List<Classifier>
        {
            lang.ClassifierByKey("key-Circle"),
            lang.ClassifierByKey("key-Line"),
            lang.ClassifierByKey("key-OffsetDuplicate"),
            lang.ClassifierByKey("key-CompositeShape"),
            lang.ClassifierByKey("key-Shape"),
        }, actual.ToList());
    }
}