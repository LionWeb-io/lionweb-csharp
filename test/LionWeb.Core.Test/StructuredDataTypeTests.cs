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

namespace LionWeb.Core.Test;

using Languages.Generated.V2024_1.SDTLang;
using M3;

[TestClass]
public class StructuredDataTypeTests
{
    [TestMethod]
    public void Empty()
    {
        var dec = new Decimal();

        Assert.ThrowsException<UnsetFieldException>(() => dec.Int);
    }

    [TestMethod]
    public void Empty_ToString()
    {
        var dec = new Decimal();

        Assert.AreEqual("Decimal { Frac = , Int =  }", dec.ToString());
    }

    [TestMethod]
    public void Empty_GetHashCode()
    {
        var dec = new Decimal();

        Assert.AreEqual(0, dec.GetHashCode());
    }

    [TestMethod]
    public void Empty_Equals()
    {
        var dec = new Decimal();

        Assert.IsTrue(dec.Equals(new Decimal{}));
    }

    [TestMethod]
    public void Empty_CollectAllSetFields()
    {
        var dec = new Decimal();

        CollectionAssert.AreEqual(new List<Field>(), dec.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Empty_Get()
    {
        var dec = new Decimal();

        Assert.ThrowsException<UnsetFieldException>(() => dec.Get(SDTLangLanguage.Instance.Decimal_int));
        Assert.ThrowsException<UnsetFieldException>(() => dec.Get(SDTLangLanguage.Instance.Decimal_frac));
    }

    [TestMethod]
    public void Partial()
    {
        var e = new E { E2f = new F("test") };

        Assert.AreEqual(new F("test"), e.E2f);
    }

    [TestMethod]
    public void Partial_ToString()
    {
        var e = new E { E2f = new F("test") };

        Assert.AreEqual("E { E2f = F { Name = test }, Name =  }", e.ToString());
    }

    [TestMethod]
    public void Partial_GetHashCode()
    {
        var e = new E { E2f = new F("test") };

        Assert.AreNotEqual(0, e.GetHashCode());
    }

    [TestMethod]
    public void Partial_Equals()
    {
        var e = new E { E2f = new F("test") };

        Assert.IsTrue(e.Equals(new E { E2f = new F("test") }));

        Assert.IsFalse(e.Equals(new E{}));
        Assert.IsFalse(e.Equals(new E { E2f = new F("hello") }));
        Assert.IsFalse(e.Equals(new E { Name = "test" }));
    }

    [TestMethod]
    public void Partial_CollectAllSetFields()
    {
        var e = new E { E2f = new F("test") };

        CollectionAssert.AreEqual(new List<Field> { SDTLangLanguage.Instance.E_e2f },
            e.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Partial_Get()
    {
        var e = new E { E2f = new F("test") };

        Assert.AreEqual(new F("test"), e.Get(SDTLangLanguage.Instance.E_e2f));
        Assert.ThrowsException<UnsetFieldException>(() => e.Get(SDTLangLanguage.Instance.E_name));
    }

    [TestMethod]
    public void Partial_Null()
    {
        var e = new E { E2f = new F("test"), Name = null };

        Assert.AreEqual("test", e.E2f.Name);
    }

    [TestMethod]
    public void Partial_Null_ToString()
    {
        var e = new E { E2f = new F("test"), Name = null };

        Assert.AreEqual("E { E2f = F { Name = test }, Name =  }", e.ToString());
    }

    [TestMethod]
    public void Partial_Null_GetHashCode()
    {
        var e = new E { E2f = new F("test"), Name = null };

        Assert.AreNotEqual(0, e.GetHashCode());
    }

    [TestMethod]
    public void Partial_Null_Equals()
    {
        var e = new E { E2f = new F("test"), Name = null };

        Assert.IsTrue(e.Equals(new E { E2f = new F("test") }));

        Assert.IsFalse(e.Equals(new E{}));
        Assert.IsFalse(e.Equals(new E { E2f = new F("hello") }));
        Assert.IsFalse(e.Equals(new E { Name = "Test" }));
    }

    [TestMethod]
    public void Partial_Null_Equals_Null()
    {
        var e = new E { E2f = new F("test"), Name = null };

        Assert.IsTrue(e.Equals(new E { E2f = new F("test"), Name = null }));

        Assert.IsFalse(e.Equals(new E{}));
        Assert.IsFalse(e.Equals(new E { E2f = new F("hello") }));
        Assert.IsFalse(e.Equals(new E { Name = "Test" }));
    }

    [TestMethod]
    public void Partial_Equals_Null()
    {
        var e = new E { E2f = new F("test") };

        Assert.IsTrue(e.Equals(new E { E2f = new F("test"), Name = null }));

        Assert.IsFalse(e.Equals(new E{}));
        Assert.IsFalse(e.Equals(new E { E2f = new F("hello") }));
        Assert.IsFalse(e.Equals(new E { Name = "Test" }));
    }

    [TestMethod]
    public void Partial_Null_CollectAllSetFields()
    {
        var e = new E { E2f = new F("test"), Name = null };

        CollectionAssert.AreEqual(new List<Field> { SDTLangLanguage.Instance.E_e2f },
            e.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Partial_Null_Get()
    {
        var e = new E { E2f = new F("test"), Name = null };

        Assert.AreEqual(new F("test"), e.Get(SDTLangLanguage.Instance.E_e2f));
        Assert.ThrowsException<UnsetFieldException>(() => e.Get(SDTLangLanguage.Instance.E_name));
    }

    [TestMethod]
    public void Complete()
    {
        var amount = new Amount
        {
            Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true
        };

        Assert.AreEqual(23, amount.Value.Int);
        Assert.AreEqual(42, amount.Value.Frac);
        Assert.AreEqual(Currency.EUR, amount.Currency);
        Assert.AreEqual(true, amount.Digital);
    }

    [TestMethod]
    public void Complete_ToString()
    {
        var amount = new Amount
        {
            Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true
        };

        Assert.AreEqual("Amount { Currency = EUR, Digital = True, Value = Decimal { Frac = 42, Int = 23 } }",
            amount.ToString());
    }

    [TestMethod]
    public void Complete_GetHashCode()
    {
        var amount = new Amount
        {
            Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true
        };

        Assert.AreNotEqual(0, amount.GetHashCode());
        Assert.AreEqual(-984265222, amount.GetHashCode());
    }

    [TestMethod]
    public void Complete_Equals()
    {
        var amount = new Amount
        {
            Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true
        };
        var amount2 = new Amount
        {
            Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true
        };

        Assert.IsTrue(amount.Equals(amount2));

        Assert.IsFalse(amount.Equals(new Amount()));
        Assert.IsFalse(amount.Equals(new Amount { Currency = Currency.EUR }));
    }

    [TestMethod]
    public void Complete_CollectAllSetFields()
    {
        var amount = new Amount
        {
            Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true
        };

        CollectionAssert.AreEqual(
            new List<Field>
            {
                SDTLangLanguage.Instance.Amount_currency,
                SDTLangLanguage.Instance.Amount_digital,
                SDTLangLanguage.Instance.Amount_value,
            }, amount.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Complete_Get()
    {
        var amount = new Amount
        {
            Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true
        };

        Assert.AreEqual(new Decimal { Int = 23, Frac = 42 }, amount.Get(SDTLangLanguage.Instance.Amount_value));
        Assert.AreEqual(Currency.EUR, amount.Get(SDTLangLanguage.Instance.Amount_currency));
        Assert.AreEqual(true, amount.Get(SDTLangLanguage.Instance.Amount_digital));
    }

    [TestMethod]
    public void ComplexStructure()
    {
        var a = new A
        {
            Name = "A1",
            A2b = new B { Name = "B1", B2d = new D { Name = "D1" } },
            A2c = new C
            {
                Name = "C1",
                C2d = new D { Name = "D2" },
                C2e = new E
                {
                    Name = "E1",
                    E2f = new F
                    {
                        Name = "F1"
                    }
                }
            }
        };

        Assert.AreEqual(
            "A { A2b = B { B2d = D { Name = D1 }, Name = B1 }, A2c = C { C2d = D { Name = D2 }, C2e = E { E2f = F { Name = F1 }, Name = E1 }, Name = C1 }, Name = A1 }",
            a.ToString());
    }
}