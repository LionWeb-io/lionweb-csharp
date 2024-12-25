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

using Examples.V2024_1.Circular.A;
using Examples.V2024_1.Circular.B;

[TestClass]
public class CircularTests
{
    [TestMethod]
    public void InstantiateWithNew()
    {
        ALangLanguage ALangLanguage = new ALangLanguage("AA");
        Assert.AreEqual("key-ALang", ALangLanguage.Key);

        AConcept aConcept = new AConcept("AA1");
        Assert.AreEqual("key-AConcept", aConcept.GetClassifier().Key);

        BLangLanguage BLangLanguage = new BLangLanguage("BB");
        Assert.AreEqual("key-BLang", BLangLanguage.Key);

        BConcept bConcept = new BConcept("BB1");
        Assert.AreEqual("key-BConcept", bConcept.GetClassifier().Key);

        bConcept.ARef = aConcept;
        Assert.AreSame(aConcept, bConcept.ARef);
        Assert.AreSame(aConcept, bConcept.Get(BLangLanguage.BConcept_ARef));
        
        aConcept.BRef = bConcept;
        Assert.AreSame(bConcept, aConcept.BRef);
        Assert.AreSame(bConcept, aConcept.Get(ALangLanguage.AConcept_BRef));
    }
    
    [TestMethod]
    public void InstantiateWithSingleton()
    {
        ALangLanguage ALangLanguage = ALangLanguage.Instance;
        Assert.AreEqual("key-ALang", ALangLanguage.Key);

        AConcept aConcept = new AConcept("AA1");
        Assert.AreEqual("key-AConcept", aConcept.GetClassifier().Key);

        BLangLanguage BLangLanguage = BLangLanguage.Instance;
        Assert.AreEqual("key-BLang", BLangLanguage.Key);

        BConcept bConcept = new BConcept("BB1");
        Assert.AreEqual("key-BConcept", bConcept.GetClassifier().Key);

        bConcept.ARef = aConcept;
        Assert.AreSame(aConcept, bConcept.ARef);
        Assert.AreSame(aConcept, bConcept.Get(BLangLanguage.BConcept_ARef));
        
        aConcept.BRef = bConcept;
        Assert.AreSame(bConcept, aConcept.BRef);
        Assert.AreSame(bConcept, aConcept.Get(ALangLanguage.AConcept_BRef));
    }
    
    [TestMethod]
    public void InstantiateWithFactory()
    {
        ALangLanguage ALangLanguage = ALangLanguage.Instance;
        Assert.AreEqual("key-ALang", ALangLanguage.Key);

        AConcept aConcept = ALangLanguage.GetFactory().CreateAConcept();
        Assert.AreEqual("key-AConcept", aConcept.GetClassifier().Key);

        BLangLanguage BLangLanguage = BLangLanguage.Instance;
        Assert.AreEqual("key-BLang", BLangLanguage.Key);

        BConcept bConcept = BLangLanguage.GetFactory().CreateBConcept();
        Assert.AreEqual("key-BConcept", bConcept.GetClassifier().Key);

        bConcept.ARef = aConcept;
        Assert.AreSame(aConcept, bConcept.ARef);
        Assert.AreSame(aConcept, bConcept.Get(BLangLanguage.BConcept_ARef));
        
        aConcept.BRef = bConcept;
        Assert.AreSame(bConcept, aConcept.BRef);
        Assert.AreSame(bConcept, aConcept.Get(ALangLanguage.AConcept_BRef));
    }
    
}