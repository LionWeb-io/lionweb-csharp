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

namespace LionWeb.Core.Test.NodeApi.Dynamic.StructuredDataType;

using M1;
using M2;
using M3;

[TestClass]
public class StructuredDataTypeTests : StructuredDataTypeTestBase
{
    #region EqualsHashCode

    [TestMethod]
    public void Identical()
    {
        var input = NewAmount(1, 2, "EUR", true);

        Assert.AreEqual(input, input);
        Assert.AreEqual(input.GetHashCode(), input.GetHashCode());
    }

    [TestMethod]
    public void Same()
    {
        var input = NewAmount(1, 2, "EUR", true);
        var same = NewAmount(1, 2, "EUR", true);

        Assert.AreEqual(input, same);
        Assert.AreEqual(input.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void NotSame_NestedSdt()
    {
        var input = NewAmount(1, 2, "EUR", true);
        var same = NewAmount(2, 2, "EUR", true);

        Assert.AreNotEqual(input, same);
        Assert.AreNotEqual(input.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void NotSame_NestedEnum()
    {
        var input = NewAmount(1, 2, "EUR", true);
        var same = NewAmount(1, 2, "GBP", true);

        Assert.AreNotEqual(input, same);
        Assert.AreNotEqual(input.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void NotSame_NestedBoolean()
    {
        var input = NewAmount(1, 2, "EUR", true);
        var same = NewAmount(1, 2, "GBP", false);

        Assert.AreNotEqual(input, same);
        Assert.AreNotEqual(input.GetHashCode(), same.GetHashCode());
    }

    #endregion

    #region Single

    [TestMethod]
    public void Reflective()
    {
        var parent = NewSdtConcept("od");
        var value = NewDecimal(1, 2);
        parent.Set(SdtConcept_decimal, value);
        Assert.AreEqual(NewDecimal(1, 2), parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = NewSdtConcept("od");
        parent.Set(SdtConcept_decimal, NewDecimal(1, 2));
        Assert.AreEqual(NewDecimal(1, 2), parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Bye_Reflective()
    {
        var parent = NewSdtConcept("od");
        var value = NewDecimal(11, 22);
        parent.Set(SdtConcept_decimal, value);
        Assert.AreEqual(NewDecimal(11, 22), parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void OtherSdt_Reflective()
    {
        var parent = NewSdtConcept("od");
        var value = NewAmount(11, 22, "EUR", true);
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(SdtConcept_decimal, value));
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = NewSdtConcept("od");
        var value = true;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(SdtConcept_decimal, value));
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = NewSdtConcept("od");
        var value = 10;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(SdtConcept_decimal, value));
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = NewSdtConcept("od");
        object? value = null;
        parent.Set(SdtConcept_decimal, value);
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = NewSdtConcept("od");
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    #endregion

    #region Loop

    [TestMethod]
    public void Loop_Reflective()
    {
        LionWebVersions lionWebVersion = LionWebVersions.v2024_1;
        var language =
            new DynamicLanguage("lang", lionWebVersion) { Key = "key-myLanguage", Version = "1", Name = "myLanguage" };
        var myConcept = language.Concept("conc", "key-myConcept", "myConcept");
        var sdt = language.StructuredDataType("sdt", "key-mySdt", "mySdt");
        var field = sdt.Field("field", "key-myField", "myField")
            .OfType(lionWebVersion.BuiltIns.String);
        sdt.Field("field2", "key-field2", "myField2")
            .OfType(sdt);
        var prop = myConcept
            .Property("id-sdt", "key-sdt", "propSdt")
            .OfType(sdt)
            .IsOptional(true);

        var chunk = new SerializerBuilder().WithLionWebVersion(lionWebVersion).Build()
            .SerializeToChunk([language]);
        Assert.ThrowsException<UnsupportedStructuredDataTypeException>(() =>
            new LanguageDeserializerBuilder().WithLionWebVersion(lionWebVersion).Build().Deserialize(chunk));
    }

    #endregion

    #region Empty

    [TestMethod]
    public void Empty()
    {
        var dec = NewDecimal();

        Assert.ThrowsException<UnsetFieldException>(() => dec.Get(DecimalInt()));
    }

    [TestMethod]
    public void Empty_ToString()
    {
        var dec = NewDecimal();

        Assert.AreEqual("Decimal { frac = , int =  }", dec.ToString());
    }

    [TestMethod]
    public void Empty_GetHashCode()
    {
        var dec = NewDecimal();

        Assert.AreNotEqual(0, dec.GetHashCode());
    }

    [TestMethod]
    public void Empty_Equals()
    {
        var dec = NewDecimal();

        Assert.IsTrue(dec.Equals(NewDecimal()));
    }

    [TestMethod]
    public void Empty_CollectAllSetFields()
    {
        var dec = NewDecimal();

        CollectionAssert.AreEqual(new List<Field>(), dec.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Empty_Get()
    {
        var dec = NewDecimal();

        Assert.ThrowsException<UnsetFieldException>(() => dec.Get(DecimalInt()));
        Assert.ThrowsException<UnsetFieldException>(() => dec.Get(DecimalFrac()));
    }

    #endregion

    #region Partial

    [TestMethod]
    public void Partial()
    {
        var name = "test";
        var e2F = NewF(name);
        var e = NewE(e2F);

        Assert.AreEqual(NewF("test"), e.Get(E_e2f()));
    }


    [TestMethod]
    public void Partial_ToString()
    {
        var e = NewE(NewF("test"));

        Assert.AreEqual("E { e2f = F { name = test }, name =  }", e.ToString());
    }

    [TestMethod]
    public void Partial_GetHashCode()
    {
        var e = NewE(NewF("test"));

        Assert.AreNotEqual(0, e.GetHashCode());
    }

    [TestMethod]
    public void Partial_Equals()
    {
        var e = NewE(NewF("test"));

        Assert.IsTrue(e.Equals(NewE(NewF("test"))));

        Assert.IsFalse(e.Equals(NewE()));
        Assert.IsFalse(e.Equals(NewE(NewF("hello"))));
        Assert.IsFalse(e.Equals(NewE("test")));
    }

    [TestMethod]
    public void Partial_CollectAllSetFields()
    {
        var e = NewE(NewF("test"));

        CollectionAssert.AreEqual(new List<Field> { E_e2f() },
            e.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Partial_Get()
    {
        var e = NewE(NewF("test"));

        Assert.AreEqual(NewF("test"), e.Get(E_e2f()));
        Assert.ThrowsException<UnsetFieldException>(() => e.Get(E_name()));
    }

    [TestMethod]
    public void Partial_Null()
    {
        var e = NewE(NewF("test"), null);

        Assert.AreEqual("test", ((IStructuredDataTypeInstance)e.Get(E_e2f())).Get(F_name()));
    }

    [TestMethod]
    public void Partial_Null_ToString()
    {
        var e = NewE(NewF("test"), null);

        // Assert.AreEqual("E { E2f = F { Name = test }, Name =  }", e.ToString());
        Assert.IsNotNull(e.ToString());
    }

    [TestMethod]
    public void Partial_Null_GetHashCode()
    {
        var e = NewE(NewF("test"), null);

        Assert.AreNotEqual(0, e.GetHashCode());
    }

    [TestMethod]
    public void Partial_Null_Equals()
    {
        var e = NewE(NewF("test"), null);

        Assert.IsTrue(e.Equals(NewE(NewF("test"))));

        Assert.IsFalse(e.Equals(NewE()));
        Assert.IsFalse(e.Equals(NewE(NewF("hello"))));
        Assert.IsFalse(e.Equals(NewE("Test")));
    }

    [TestMethod]
    public void Partial_Null_Equals_Null()
    {
        var e = NewE(NewF("test"), null);

        Assert.IsTrue(e.Equals(NewE(NewF("test"), null)));

        Assert.IsFalse(e.Equals(NewE()));
        Assert.IsFalse(e.Equals(NewE(NewF("hello"))));
        Assert.IsFalse(e.Equals(NewE("Test")));
    }

    [TestMethod]
    public void Partial_Equals_Null()
    {
        var e = NewE(NewF("test"));

        Assert.IsTrue(e.Equals(NewE(NewF("test"), null)));

        Assert.IsFalse(e.Equals(NewE()));
        Assert.IsFalse(e.Equals(NewE(NewF("hello"))));
        Assert.IsFalse(e.Equals(NewE("Test")));
    }

    [TestMethod]
    public void Partial_Null_CollectAllSetFields()
    {
        var e = NewE(NewF("test"), null);

        CollectionAssert.AreEqual(new List<Field> { E_e2f() },
            e.CollectAllSetFields().ToList());
    }

    [TestMethod]
    public void Partial_Null_Get()
    {
        var e = NewE(NewF("test"), null);

        Assert.AreEqual(NewF("test"), e.Get(E_e2f()));
        Assert.ThrowsException<UnsetFieldException>(() => e.Get(E_name()));
    }

    #endregion

    #region Complete

    [TestMethod]
    public void Complete()
    {
        var amount = NewAmount(23, 42, "EUR", true);

        Assert.AreEqual(23, ((IStructuredDataTypeInstance)amount.Get(Amount_value())).Get(DecimalInt()));
        Assert.AreEqual(42, ((IStructuredDataTypeInstance)amount.Get(Amount_value())).Get(DecimalFrac()));
        Assert.AreEqual(CurrencyEUR(), amount.Get(Amount_currency()));
        Assert.AreEqual(true, amount.Get(Amount_digital()));
    }

    [TestMethod]
    public void Complete_ToString()
    {
        var amount = NewAmount(23, 42, "EUR", true);

        Assert.AreEqual("Amount { currency = EUR, digital = True, value = Decimal { frac = 42, int = 23 } }",
            amount.ToString());
    }

    [TestMethod]
    public void Complete_GetHashCode()
    {
        var amount = NewAmount(23, 42, "EUR", true);

        Assert.AreNotEqual(0, amount.GetHashCode());
    }

    [TestMethod]
    public void Complete_Equals()
    {
        var amount = NewAmount(23, 42, "EUR", true);
        var amount2 = NewAmount(23, 42, "EUR", true);

        Assert.IsTrue(amount.Equals(amount2));

        Assert.IsFalse(amount.Equals(NewAmount()));
        Assert.IsFalse(amount.Equals(NewAmount(CurrencyEUR())));
    }

    [TestMethod]
    public void Complete_CollectAllSetFields()
    {
        var amount = NewAmount(23, 42, "EUR", true);

        var expected = new List<Field> { Amount_value(), Amount_currency(), Amount_digital() };
        List<Field> actual = amount.CollectAllSetFields().ToList();
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Complete_Get()
    {
        var amount = NewAmount(23, 42, "EUR", true);

        Assert.AreEqual(NewDecimal(23, 42), amount.Get(Amount_value()));
        Assert.AreEqual(CurrencyEUR(), amount.Get(Amount_currency()));
        Assert.AreEqual(true, amount.Get(Amount_digital()));
    }

    #endregion
}