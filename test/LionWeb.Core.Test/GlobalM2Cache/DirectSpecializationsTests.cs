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
public class DirectSpecializationsTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void LeafInterface()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidI4")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleInterface()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidI1")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-ValidI0")! }, actual.ToList());
    }

    [TestMethod]
    public void MultipleInterface()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidI2")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-ValidI1")!, cache.FindByKey<Classifier>(lang, "key-ValidI3")! },
            actual.ToList());
    }

    [TestMethod]
    public void DirectInterfaceCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-IA")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-IB")!, }, actual.ToList());
    }

    [TestMethod]
    public void InterfaceWithImplementations()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidI3")!)!;

        CollectionAssert.AreEquivalent(
            new List<Classifier>
            {
                cache.FindByKey<Classifier>(lang, "key-ValidI0")!,
                cache.FindByKey<Classifier>(lang, "key-ValidC1")!,
                cache.FindByKey<Classifier>(lang, "key-ValidA1")!
            }, actual.ToList());
    }

    [TestMethod]
    public void LeafConcept()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidC0")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleConcept()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidC1")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-ValidC0")! }, actual.ToList());
    }

    [TestMethod]
    public void ConceptLine()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidC2")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-ValidC1")! }, actual.ToList());
    }

    [TestMethod]
    public void DirectConceptCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-CA")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-CB")!, }, actual.ToList());
    }

    [TestMethod]
    public void LeafAnnotation()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidA0")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { }, actual.ToList());
    }

    [TestMethod]
    public void SingleAnnotation()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidA1")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-ValidA0")! }, actual.ToList());
    }

    [TestMethod]
    public void AnnotationLine()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-ValidA2")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-ValidA1")! }, actual.ToList());
    }

    [TestMethod]
    public void DirectAnnotationCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.DirectSpecializations(cache.FindByKey<Classifier>(lang, "key-AA")!)!;

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-AB")!, }, actual.ToList());
    }

    [TestMethod]
    public void CrossLanguage()
    {
        var cache = new M2Cache();
        Language lang = ShapesDynamic.Language;
        cache.Register([lang, InvalidLanguage.Language]);

        var actual = _builtIns.INamed.DirectSpecializations([lang, InvalidLanguage.Language]);

        CollectionAssert.AreEquivalent(new List<Classifier> { cache.FindByKey<Classifier>(lang, "key-Shape")!, cache.FindByKey<Classifier>(lang, "key-Line")!, }, actual.ToList());
    }
}