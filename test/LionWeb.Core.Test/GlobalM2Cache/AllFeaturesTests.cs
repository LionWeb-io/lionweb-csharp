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
public class AllFeaturesTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void Interface()
    {
        var cache = new M2Cache();
        cache.Register([_builtIns]);

        var actual = cache.AllFeatures(_builtIns.INamed)!;

        CollectionAssert.AreEquivalent(
            new List<Feature> { _builtIns.INamed_name }, actual.ToList());
    }

    [TestMethod]
    public void RootInterface()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-ValidI4")!)!;

        CollectionAssert.AreEquivalent(new List<Feature> { }, actual.ToList());
    }

    [TestMethod]
    public void IndirectInterfaceCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-ID")!)!;

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-IC")!, "key-IC-propA")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-ID")!, "key-ID-propA")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-IE")!, "key-IE-propA")!,
            }, actual.ToList());
    }

    [TestMethod]
    public void Concept()
    {
        var cache = new M2Cache();
        Language lang = ShapesDynamic.Language;
        cache.Register([_builtIns, lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-Line")!)!;

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                _builtIns.INamed_name!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-Shape")!, "key-shape-docs")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-Line")!, "key-start")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-Line")!, "key-end")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-IShape")!, "key-uuid")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-IShape")!, "key-fixpoints")!,
            }, actual.ToList());
    }

    [TestMethod]
    public void RootConcept()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-ValidC3")!)!;

        CollectionAssert.AreEquivalent(new List<Feature> { }, actual.ToList());
    }

    [TestMethod]
    public void IndirectConceptCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-CD")!)!;

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-CC")!, "key-CC-refA")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-CD")!, "key-CD-refA")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-CE")!, "key-CE-refA")!,
            }, actual.ToList());
    }

    [TestMethod]
    public void Annotation()
    {
        var cache = new M2Cache();
        Language lang = ShapesDynamic.Language;
        cache.Register([lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-Documentation")!)!;

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-Documentation")!, "key-text")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-Documentation")!, "key-technical")!
            }, actual.ToList());
    }

    [TestMethod]
    public void RootAnnotation()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-ValidA3")!)!;

        CollectionAssert.AreEquivalent(new List<Feature> { }, actual.ToList());
    }

    [TestMethod]
    public void IndirectAnnotationCircle()
    {
        var cache = new M2Cache();
        Language lang = InvalidLanguage.Language;
        cache.Register([lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-AD")!)!;

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-AC")!, "key-AC-contA")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-AD")!, "key-AD-contA")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-AE")!, "key-AE-contA")!,
            }, actual.ToList());
    }

    [TestMethod]
    public void CrossLanguage()
    {
        var cache = new M2Cache();
        Language lang = ShapesDynamic.Language;
        cache.Register([_builtIns, lang]);

        var actual = cache.AllFeatures(cache.FindByKey<Classifier>(lang, "key-Circle")!)!;

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                _builtIns.INamed_name,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-Shape")!, "key-shape-docs")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-Circle")!, "key-r")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-Circle")!, "key-center")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-IShape")!, "key-uuid")!,
                cache.FeatureByKey(cache.FindByKey<Classifier>(lang, "key-IShape")!, "key-fixpoints")!,
            },
            actual.ToList());
    }
}