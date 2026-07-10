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

namespace LionWeb.Core.Test.GlobalM2Cache;

using Languages.Generated.V2024_1.SDTLang;
using M2;
using M3;
using System.Collections.Immutable;

[TestClass]
public class UsedByM2ExtensionsTests : IDisposable
{
    private class TestGlobalM2Cache : IGlobalM2Cache
    {
        public bool _findByKey;
        public bool _allFeatures;
        public bool _allGeneralizations;
        public bool _allSpecializations;
        public bool _directGeneralizations;
        public bool _directSpecializations;
        public bool _featureByKey;
        public bool _fieldByKey;
        public bool _literalByKey;

        public T? FindByKey<T>(Language language, string key) where T : class, IKeyed
        {
            _findByKey = true;
            return null;
        }

        public IImmutableSet<Feature>? AllFeatures(Classifier classifier)
        {
            _allFeatures = true;
            return null;
        }

        public IImmutableSet<Classifier>? AllGeneralizations(Classifier classifier)
        {
            _allGeneralizations = true;
            return null;
        }

        public IImmutableSet<Classifier>? AllSpecializations(Classifier classifier)
        {
            _allSpecializations = true;
            return null;
        }

        public IImmutableSet<Classifier>? DirectGeneralizations(Classifier classifier)
        {
            _directGeneralizations = true;
            return null;
        }

        public IImmutableSet<Classifier>? DirectSpecializations(Classifier classifier)
        {
            _directSpecializations = true;
            return null;
        }

        public Feature? FeatureByKey(Classifier classifier, string key)
        {
            _featureByKey = true;
            return null;
        }

        public Field? FieldByKey(StructuredDataType structuredDataType, string key)
        {
            _fieldByKey = true;
            return null;
        }

        public EnumerationLiteral? LiteralByKey(Enumeration enumeration, string key)
        {
            _literalByKey = true;
            return null;
        }

        public static void EnableTest() => IGlobalM2Cache.Instance = new TestGlobalM2Cache();
        
        public void Clear() => throw new NotImplementedException();

        public void Register(IEnumerable<Language> languages) => throw new NotImplementedException();
    }

    private static readonly SDTLangLanguage _sdtLang = SDTLangLanguage.Instance; 
    
    public UsedByM2ExtensionsTests()
    {
        TestGlobalM2Cache.EnableTest();
    }

    public void Dispose() =>
        IGlobalM2Cache.Disable();

    [TestMethod]
    public void FindByKey()
    {
        _sdtLang.FindByKey<StructuredDataType>(_sdtLang.A.Key);
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._findByKey);
    }
    
    [TestMethod]
    public void ClassifierByKey()
    {
        _sdtLang.ClassifierByKey(_sdtLang.SDTConcept.Key);
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._findByKey);
    }
    
    [TestMethod]
    public void FeatureByKey()
    {
        _sdtLang.SDTConcept.FeatureByKey(_sdtLang.SDTConcept_A.Key);
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._featureByKey);
    }
    
    [TestMethod]
    public void AllFeatures()
    {
        _sdtLang.SDTConcept.AllFeatures();
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._allFeatures);
    }
    
    [TestMethod]
    public void DirectGeneralizations()
    {
        ((Classifier)_sdtLang.SDTConcept).DirectGeneralizations();
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._directGeneralizations);
    }
    
    [TestMethod]
    public void AllGeneralizations()
    {
        _sdtLang.SDTConcept.AllGeneralizations();
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._allGeneralizations);
    }
    
    [TestMethod]
    public void DirectSpecializations()
    {
        _sdtLang.SDTConcept.DirectSpecializations([_sdtLang]);
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._directSpecializations);
    }
    
    [TestMethod]
    public void AllSpecializations()
    {
        _sdtLang.SDTConcept.AllSpecializations([_sdtLang]);
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._allSpecializations);
    }
    
    [TestMethod]
    public void StructuredDataTypeByKey()
    {
        _sdtLang.StructuredDataTypeByKey(_sdtLang.A.Key);
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._findByKey);
    }
    
    [TestMethod]
    public void FieldByKey()
    {
        _sdtLang.A.FieldByKey(_sdtLang.A_a2b.Key);
        Assert.IsTrue(((TestGlobalM2Cache)IGlobalM2Cache.Instance!)._fieldByKey);
    }
}