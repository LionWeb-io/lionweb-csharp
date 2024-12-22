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

using Examples.V2024_1.SDTLang;
using LionWeb.Core;
using LionWeb.Core.M3;

[TestClass]
public class StructuredDataTypeTests
{
    [TestMethod]
    public void Empty()
    {
        var fqn = new FullyQualifiedName();

        Assert.ThrowsException<UnsetFieldException>(() => fqn.Name);
    }

    [TestMethod]
    public void Empty_ToString()
    {
        var fqn = new FullyQualifiedName();

        Assert.ThrowsException<UnsetFieldException>(() => fqn.ToString());
    }

    [TestMethod]
    public void Empty_GetHashCode()
    {
        var fqn = new FullyQualifiedName();

        Assert.AreEqual(0, fqn.GetHashCode());
    }

    [TestMethod]
    public void Empty_Equals()
    {
        var fqn = new FullyQualifiedName();

        Assert.IsTrue(fqn.Equals(new FullyQualifiedName{}));
    }

    [TestMethod]
    public void Empty_CollectAllSetFields()
    {
        var fqn = new FullyQualifiedName();

        CollectionAssert.AreEqual(new List<Field>(), fqn.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Empty_Get()
    {
        var fqn = new FullyQualifiedName();

        Assert.ThrowsException<UnsetFieldException>(() => fqn.Get(SDTLangLanguage.Instance.FullyQualifiedName_name));
        Assert.ThrowsException<UnsetFieldException>(() => fqn.Get(SDTLangLanguage.Instance.FullyQualifiedName_nested));
    }

    [TestMethod]
    public void Partial()
    {
        var fqn = new FullyQualifiedName { Name = "test" };

        Assert.AreEqual("test", fqn.Name);
    }

    [TestMethod]
    public void Partial_ToString()
    {
        var fqn = new FullyQualifiedName { Name = "test" };

        Assert.AreEqual("FullyQualifiedName { Name = test, Nested =  }", fqn.ToString());
    }

    [TestMethod]
    public void Partial_GetHashCode()
    {
        var fqn = new FullyQualifiedName { Name = "test" };

        Assert.AreNotEqual(0, fqn.GetHashCode());
    }

    [TestMethod]
    public void Partial_Equals()
    {
        var fqn = new FullyQualifiedName { Name = "test" };

        Assert.IsTrue(fqn.Equals(new FullyQualifiedName { Name = "test" }));

        Assert.IsFalse(fqn.Equals(new FullyQualifiedName{}));
        Assert.IsFalse(fqn.Equals(new FullyQualifiedName { Nested = new FullyQualifiedName{} }));
        Assert.IsFalse(fqn.Equals(new FullyQualifiedName { Name = "Test" }));
    }

    [TestMethod]
    public void Partial_CollectAllSetFields()
    {
        var fqn = new FullyQualifiedName { Name = "test" };

        CollectionAssert.AreEqual(new List<Field> { SDTLangLanguage.Instance.FullyQualifiedName_name },
            fqn.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Partial_Get()
    {
        var fqn = new FullyQualifiedName { Name = "test" };

        Assert.AreEqual("test", fqn.Get(SDTLangLanguage.Instance.FullyQualifiedName_name));
        Assert.ThrowsException<UnsetFieldException>(() => fqn.Get(SDTLangLanguage.Instance.FullyQualifiedName_nested));
    }

    [TestMethod]
    public void Partial_Null()
    {
        var fqn = new FullyQualifiedName { Name = "test", Nested = null };

        Assert.AreEqual("test", fqn.Name);
    }

    [TestMethod]
    public void Partial_Null_ToString()
    {
        var fqn = new FullyQualifiedName { Name = "test", Nested = null };

        Assert.AreEqual("FullyQualifiedName { Name = test, Nested =  }", fqn.ToString());
    }

    [TestMethod]
    public void Partial_Null_GetHashCode()
    {
        var fqn = new FullyQualifiedName { Name = "test", Nested = null };

        Assert.AreNotEqual(0, fqn.GetHashCode());
    }

    [TestMethod]
    public void Partial_Null_Equals()
    {
        var fqn = new FullyQualifiedName { Name = "test", Nested = null };

        Assert.IsTrue(fqn.Equals(new FullyQualifiedName { Name = "test" }));

        Assert.IsFalse(fqn.Equals(new FullyQualifiedName{}));
        Assert.IsFalse(fqn.Equals(new FullyQualifiedName { Nested = new FullyQualifiedName{} }));
        Assert.IsFalse(fqn.Equals(new FullyQualifiedName { Name = "Test" }));
    }

    [TestMethod]
    public void Partial_Null_Equals_Null()
    {
        var fqn = new FullyQualifiedName { Name = "test", Nested = null };

        Assert.IsTrue(fqn.Equals(new FullyQualifiedName { Name = "test", Nested = null }));

        Assert.IsFalse(fqn.Equals(new FullyQualifiedName{}));
        Assert.IsFalse(fqn.Equals(new FullyQualifiedName { Nested = new FullyQualifiedName{} }));
        Assert.IsFalse(fqn.Equals(new FullyQualifiedName { Name = "Test" }));
    }

    [TestMethod]
    public void Partial_Equals_Null()
    {
        var fqn = new FullyQualifiedName { Name = "test" };

        Assert.IsTrue(fqn.Equals(new FullyQualifiedName { Name = "test", Nested = null }));

        Assert.IsFalse(fqn.Equals(new FullyQualifiedName{}));
        Assert.IsFalse(fqn.Equals(new FullyQualifiedName { Nested = new FullyQualifiedName{} }));
        Assert.IsFalse(fqn.Equals(new FullyQualifiedName { Name = "Test" }));
    }

    [TestMethod]
    public void Partial_Null_CollectAllSetFields()
    {
        var fqn = new FullyQualifiedName { Name = "test", Nested = null };

        CollectionAssert.AreEqual(new List<Field> { SDTLangLanguage.Instance.FullyQualifiedName_name },
            fqn.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Partial_Null_Get()
    {
        var fqn = new FullyQualifiedName { Name = "test", Nested = null };

        Assert.AreEqual("test", fqn.Get(SDTLangLanguage.Instance.FullyQualifiedName_name));
        Assert.ThrowsException<UnsetFieldException>(() => fqn.Get(SDTLangLanguage.Instance.FullyQualifiedName_nested));
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
                SDTLangLanguage.Instance.Amount_value,
                SDTLangLanguage.Instance.Amount_currency,
                SDTLangLanguage.Instance.Amount_digital,
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
    public void Nested()
    {
        var fqn = new FullyQualifiedName
        {
            Name = "a",
            Nested = new FullyQualifiedName { Name = "b", Nested = new FullyQualifiedName { Name = "c" } }
        };

        Assert.AreEqual(
            "FullyQualifiedName { Name = a, Nested = FullyQualifiedName { Name = b, Nested = FullyQualifiedName { Name = c, Nested =  } } }",
            fqn.ToString());
    }

    [TestMethod]
    public void Nested_Null()
    {
        var fqn = new FullyQualifiedName
        {
            Name = "a",
            Nested = new FullyQualifiedName
            {
                Name = "b", Nested = new FullyQualifiedName { Name = "c", Nested = null }
            }
        };

        Assert.AreEqual(
            "FullyQualifiedName { Name = a, Nested = FullyQualifiedName { Name = b, Nested = FullyQualifiedName { Name = c, Nested =  } } }",
            fqn.ToString());
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
                    E2b = new B { Name = "B2", B2d = new D { Name = "D3" } },
                    E2f = new F
                    {
                        Name = "F1",
                        F2c = new C
                        {
                            Name = "C2",
                            C2d = new D { Name = "D4" },
                            C2e = new E
                            {
                                Name = "E2",
                                E2b = new B { Name = "B3", B2d = new D { Name = "D5" } }
                            }
                        }
                    }
                }
            }
        };

        Assert.AreEqual(
            "A { A2b = B { B2d = D { Name = D1 }, Name = B1 }, A2c = C { C2d = D { Name = D2 }, C2e = E { E2b = B { B2d = D { Name = D3 }, Name = B2 }, E2f = F { F2c = C { C2d = D { Name = D4 }, C2e = E { E2b = B { B2d = D { Name = D5 }, Name = B3 }, E2f = , Name = E2 }, Name = C2 }, Name = F1 }, Name = E1 }, Name = C1 }, Name = A1 }",
            a.ToString());
    }
}