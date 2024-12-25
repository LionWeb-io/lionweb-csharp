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

namespace LionWeb.Core.Test.NodeApi;

using Languages;
using M2;
using M3;

[TestClass]
public class DirectGeneralizationsTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void RootInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI4").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI1").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidI2") }, actual.ToList());
    }

    [TestMethod]
    public void MultipleInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI0").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidI1"), lang.ClassifierByKey("key-ValidI3") },
            actual.ToList());
    }

    [TestMethod]
    public void InterfaceLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI8").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidI7") },
            actual.ToList());
    }

    [TestMethod]
    public void DirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-IA").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-IB"), }, actual.ToList());
    }

    [TestMethod]
    public void RootConcept()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC3").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void ConceptLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC0").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidC1") }, actual.ToList());
    }

    [TestMethod]
    public void DirectConceptCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-CA").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-CB"), }, actual.ToList());
    }

    [TestMethod]
    public void ConceptSingleInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC2").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidI0") }, actual.ToList());
    }

    [TestMethod]
    public void ConceptMultipleInterfaces()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC4").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidI5"), lang.ClassifierByKey("key-ValidI6"), },
            actual.ToList());
    }

    [TestMethod]
    public void ConceptDirectAndInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC1").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidC2"), lang.ClassifierByKey("key-ValidI3"), },
            actual.ToList());
    }

    [TestMethod]
    public void ConceptInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-CF").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-IJ"), lang.ClassifierByKey("key-IL"), },
            actual.ToList());
    }

    [TestMethod]
    public void RootAnnotation()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA3").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void AnnotationLine()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA0").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidA1") }, actual.ToList());
    }

    [TestMethod]
    public void DirectAnnotationCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-AA").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-AB"), }, actual.ToList());
    }

    [TestMethod]
    public void AnnotationSingleInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC2").DirectGeneralizations();

        CollectionAssert.AreEquivalent(new List<Classifier> { lang.ClassifierByKey("key-ValidI0") }, actual.ToList());
    }

    [TestMethod]
    public void AnnotationMultipleInterfaces()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA4").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidI5"), lang.ClassifierByKey("key-ValidI6"), },
            actual.ToList());
    }

    [TestMethod]
    public void AnnotationDirectAndInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA1").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-ValidA2"), lang.ClassifierByKey("key-ValidI3"), },
            actual.ToList());
    }

    [TestMethod]
    public void AnnotationInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-AF").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { lang.ClassifierByKey("key-IJ"), lang.ClassifierByKey("key-IL"), },
            actual.ToList());
    }

    [TestMethod]
    public void CrossLanguage()
    {
        Language lang = ShapesDynamic.Language;

        var actual = lang.ClassifierByKey("key-Shape").DirectGeneralizations();

        CollectionAssert.AreEquivalent(
            new List<Classifier> { _builtIns.INamed, lang.ClassifierByKey("key-IShape") },
            actual.ToList());
    }
}