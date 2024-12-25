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

namespace LionWeb.Core.M2.Generated.Test;

using Examples.V2024_1.SDTLang;
using M3;

[TestClass]
public class StructuredDataTypesTests
{
    #region EqualsHashCode

    [TestMethod]
    public void Identical()
    {
        var input = newAmount(1, 2, "EUR", true);

        Assert.AreEqual(input, input);
        Assert.AreEqual(input.GetHashCode(), input.GetHashCode());
    }

    [TestMethod]
    public void Same()
    {
        var input = newAmount(1, 2, "EUR", true);
        var same = newAmount(1, 2, "EUR", true);

        Assert.AreEqual(input, same);
        Assert.AreEqual(input.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void NotSame_NestedSdt()
    {
        var input = newAmount(1, 2, "EUR", true);
        var same = newAmount(2, 2, "EUR", true);

        Assert.AreNotEqual(input, same);
        Assert.AreNotEqual(input.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void NotSame_NestedEnum()
    {
        var input = newAmount(1, 2, "EUR", true);
        var same = newAmount(1, 2, "GBP", true);

        Assert.AreNotEqual(input, same);
        Assert.AreNotEqual(input.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void NotSame_NestedBoolean()
    {
        var input = newAmount(1, 2, "EUR", true);
        var same = newAmount(1, 2, "GBP", false);

        Assert.AreNotEqual(input, same);
        Assert.AreNotEqual(input.GetHashCode(), same.GetHashCode());
    }

    #endregion

    #region Single

    [TestMethod]
    public void Reflective()
    {
        var parent = newSdtConcept("od");
        var value = newDecimal(1, 2);
        parent.Set(SdtConcept_decimal, value);
        Assert.AreEqual(newDecimal(1, 2), parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = newSdtConcept("od");
        parent.Set(SdtConcept_decimal, newDecimal(1, 2));
        Assert.AreEqual(newDecimal(1, 2), parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Bye_Reflective()
    {
        var parent = newSdtConcept("od");
        var value = newDecimal(11, 22);
        parent.Set(SdtConcept_decimal, value);
        Assert.AreEqual(newDecimal(11, 22), parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void OtherSdt_Reflective()
    {
        var parent = newSdtConcept("od");
        var value = newAmount(11, 22, "EUR", true);
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(SdtConcept_decimal, value));
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = newSdtConcept("od");
        var value = true;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(SdtConcept_decimal, value));
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = newSdtConcept("od");
        var value = 10;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(SdtConcept_decimal, value));
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newSdtConcept("od");
        object? value = null;
        parent.Set(SdtConcept_decimal, value);
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = newSdtConcept("od");
        Assert.AreEqual(null, parent.Get(SdtConcept_decimal));
    }

    #endregion

    private SDTConcept newSdtConcept(string id) => new SDTConcept(id);

    private Property SdtConcept_decimal => SDTLangLanguage.Instance.SDTConcept_decimal;

    private IStructuredDataTypeInstance newDecimal(int dec, int frac) => new Decimal(dec, frac);

    private IStructuredDataTypeInstance newAmount(int dec, int frac, string currency, bool digital)
        => new Amount(
            Enum.TryParse<Currency>(currency, out var result) ? result : null,
            digital,
            new Decimal(dec, frac)
        );
}