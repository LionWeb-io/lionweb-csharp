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

namespace LionWeb.Core.Test.GlobalM2Cache;

using Languages;
using M2;
using M3;

[TestClass]
public class AllGeneralizationsTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void RootInterface()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidI4")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleInterface()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidI1")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-ValidI2")! }, actual.ToList());
    }

    [TestMethod]
    public void MultipleInterface()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidI7")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-ValidI5")!, cache.FindByKey<Classifier>(lang, "key-ValidI6")! },
            actual.ToList());
    }

    [TestMethod]
    public void InterfaceLine()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidI8")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                cache.FindByKey<Classifier>(lang, "key-ValidI7")!,
                cache.FindByKey<Classifier>(lang, "key-ValidI6")!,
                cache.FindByKey<Classifier>(lang, "key-ValidI5")!,
            },
            actual.ToList());
    }


    [TestMethod]
    public void DirectInterfaceCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-IA")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-IB")!, }, actual.ToList());
    }

    [TestMethod]
    public void IndirectInterfaceCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ID")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-IC")!, cache.FindByKey<Classifier>(lang, "key-IE")!, }, actual.ToList());
    }

    [TestMethod]
    public void SecondaryDirectInterfaceCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-IF")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-IG")!, cache.FindByKey<Classifier>(lang, "key-IH")!, }, actual.ToList());
    }

    [TestMethod]
    public void DiamondInterfaceCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-IJ")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                cache.FindByKey<Classifier>(lang, "key-II")!, cache.FindByKey<Classifier>(lang, "key-IK")!, cache.FindByKey<Classifier>(lang, "key-IL")!
            }, actual.ToList());
    }

    [TestMethod]
    public void ValidInterfaceDiamond()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidI0")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                cache.FindByKey<Classifier>(lang, "key-ValidI1")!,
                cache.FindByKey<Classifier>(lang, "key-ValidI2")!,
                cache.FindByKey<Classifier>(lang, "key-ValidI3")!
            }, actual.ToList());
    }

    [TestMethod]
    public void RootConcept()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidC3")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void DirectConceptCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-CA")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-CB")!, }, actual.ToList());
    }

    [TestMethod]
    public void IndirectConceptCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-CD")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-CC")!, cache.FindByKey<Classifier>(lang, "key-CE")!, }, actual.ToList());
    }

    [TestMethod]
    public void ValidConceptLine()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidC0")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier>
        {
            cache.FindByKey<Classifier>(lang, "key-ValidC1")!,
            cache.FindByKey<Classifier>(lang, "key-ValidC2")!,
            cache.FindByKey<Classifier>(lang, "key-ValidI0")!,
            cache.FindByKey<Classifier>(lang, "key-ValidI1")!,
            cache.FindByKey<Classifier>(lang, "key-ValidI2")!,
            cache.FindByKey<Classifier>(lang, "key-ValidI3")!,
        }, actual.ToList());
    }

    [TestMethod]
    public void RootAnnotation()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidA3")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void DirectAnnotationCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-AA")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-AB")!, }, actual.ToList());
    }

    [TestMethod]
    public void IndirectAnnotationCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-AD")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-AC")!, cache.FindByKey<Classifier>(lang, "key-AE")!, }, actual.ToList());
    }

    [TestMethod]
    public void ValidAnnotationLine()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-ValidA0")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier>
        {
            cache.FindByKey<Classifier>(lang, "key-ValidA1")!,
            cache.FindByKey<Classifier>(lang, "key-ValidA2")!,
            cache.FindByKey<Classifier>(lang, "key-ValidI0")!,
            cache.FindByKey<Classifier>(lang, "key-ValidI1")!,
            cache.FindByKey<Classifier>(lang, "key-ValidI2")!,
            cache.FindByKey<Classifier>(lang, "key-ValidI3")!,
        }, actual.ToList());
    }

    [TestMethod]
    public void CrossLanguage()
    {
        var cache = new M2Cache();
        Language lang = ShapesDynamic.Language;
        cache.Register([lang]);

        var actual = cache.AllGeneralizations(cache.FindByKey<Classifier>(lang, "key-Circle")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                _builtIns.INamed,
                cache.FindByKey<Classifier>(lang, "key-Shape")!,
                cache.FindByKey<Classifier>(lang, "key-IShape")!
            },
            actual.ToList());
    }
}