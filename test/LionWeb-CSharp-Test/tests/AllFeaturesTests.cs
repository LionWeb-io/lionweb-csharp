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
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;

[TestClass]
public class AllFeaturesTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void Interface()
    {
        var actual = _builtIns.INamed.AllFeatures();

        CollectionAssert.AreEquivalent(
            new List<Feature> { _builtIns.INamed_name }, actual.ToList());
    }

    [TestMethod]
    public void RootInterface()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidI4").AllFeatures();

        CollectionAssert.AreEquivalent(new List<Feature> { }, actual.ToList());
    }

    [TestMethod]
    public void IndirectInterfaceCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ID").AllFeatures();

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                lang.ClassifierByKey("key-IC").FeatureByKey("key-IC-propA"),
                lang.ClassifierByKey("key-ID").FeatureByKey("key-ID-propA"),
                lang.ClassifierByKey("key-IE").FeatureByKey("key-IE-propA"),
            }, actual.ToList());
    }

    [TestMethod]
    public void Concept()
    {
        Language lang = ShapesDynamic.Language;

        var actual = lang.ClassifierByKey("key-Line").AllFeatures();

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                _builtIns.INamed_name,
                lang.ClassifierByKey("key-Shape").FeatureByKey("key-shape-docs"),
                lang.ClassifierByKey("key-Line").FeatureByKey("key-start"),
                lang.ClassifierByKey("key-Line").FeatureByKey("key-end"),
                lang.ClassifierByKey("key-IShape").FeatureByKey("key-uuid"),
                lang.ClassifierByKey("key-IShape").FeatureByKey("key-fixpoints"),
            }, actual.ToList());
    }

    [TestMethod]
    public void RootConcept()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidC3").AllFeatures();

        CollectionAssert.AreEquivalent(new List<Feature> { }, actual.ToList());
    }

    [TestMethod]
    public void IndirectConceptCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-CD").AllFeatures();

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                lang.ClassifierByKey("key-CC").FeatureByKey("key-CC-refA"),
                lang.ClassifierByKey("key-CD").FeatureByKey("key-CD-refA"),
                lang.ClassifierByKey("key-CE").FeatureByKey("key-CE-refA"),
            }, actual.ToList());
    }

    [TestMethod]
    public void Annotation()
    {
        Language lang = ShapesDynamic.Language;

        var actual = lang.ClassifierByKey("key-Documentation").AllFeatures();

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                lang.ClassifierByKey("key-Documentation").FeatureByKey("key-text"),
                lang.ClassifierByKey("key-Documentation").FeatureByKey("key-technical")
            }, actual.ToList());
    }

    [TestMethod]
    public void RootAnnotation()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-ValidA3").AllFeatures();

        CollectionAssert.AreEquivalent(new List<Feature> { }, actual.ToList());
    }

    [TestMethod]
    public void IndirectAnnotationCircle()
    {
        Language lang = InvalidLanguage.Language;

        var actual = lang.ClassifierByKey("key-AD").AllFeatures();

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                lang.ClassifierByKey("key-AC").FeatureByKey("key-AC-contA"),
                lang.ClassifierByKey("key-AD").FeatureByKey("key-AD-contA"),
                lang.ClassifierByKey("key-AE").FeatureByKey("key-AE-contA"),
            }, actual.ToList());
    }

    [TestMethod]
    public void CrossLanguage()
    {
        Language lang = ShapesDynamic.Language;

        var actual = lang.ClassifierByKey("key-Circle").AllFeatures();

        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                _builtIns.INamed_name,
                lang.ClassifierByKey("key-Shape").FeatureByKey("key-shape-docs"),
                lang.ClassifierByKey("key-Circle").FeatureByKey("key-r"),
                lang.ClassifierByKey("key-Circle").FeatureByKey("key-center"),
                lang.ClassifierByKey("key-IShape").FeatureByKey("key-uuid"),
                lang.ClassifierByKey("key-IShape").FeatureByKey("key-fixpoints"),
            },
            actual.ToList());
    }
}