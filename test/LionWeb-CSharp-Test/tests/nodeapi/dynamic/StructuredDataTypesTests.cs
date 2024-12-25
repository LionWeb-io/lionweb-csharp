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

namespace LionWeb.Core.M2.Dynamic.Test;

using M1;
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

        var chunk = new Serializer(lionWebVersion).SerializeToChunk([language]);
        Assert.ThrowsException<UnsupportedStructuredDataTypeException>(() =>
            new LanguageDeserializer(lionWebVersion).Deserialize(chunk));
    }

    #endregion

    private DynamicLanguage _sdtLang;

    [TestInitialize]
    public void LoadSdtLanguage()
    {
        _sdtLang = LanguagesUtils
            .LoadLanguages("LionWeb-CSharp-Test", "LionWeb_CSharp_Test.languages.defChunks.sdtLang.json",
                LionWebVersions.v2024_1)
            .First();
    }

    private DynamicNode newSdtConcept(string id) =>
        _sdtLang.GetFactory().CreateNode(id, _sdtLang.ClassifierByKey("key-SDTConcept")) as DynamicNode ??
        throw new AssertFailedException();

    private Property SdtConcept_decimal =>
        _sdtLang.ClassifierByKey("key-SDTConcept").FeatureByKey("key-SDTDecimalField") as Property ??
        throw new AssertFailedException();

    private IStructuredDataTypeInstance newDecimal(int dec, int frac)
    {
        var decSdt = _sdtLang.StructuredDataTypeByKey("key-SDTDecimal");
        var decValue = _sdtLang.GetFactory().CreateStructuredDataTypeInstance(decSdt,
            new FieldValues { { decSdt.FieldByKey("key-SDTInt"), dec }, { decSdt.FieldByKey("key-SDTFrac"), frac }, }
        );

        return decValue;
    }

    private IStructuredDataTypeInstance newAmount(int dec, int frac, string currency, bool digital)
    {
        var decValue = newDecimal(dec, frac);

        var currencyEnum = _sdtLang.Enumerations().FirstOrDefault(c => c.Key == "key-SDTCurrency")!;
        var currencyLit = currencyEnum.Literals.FirstOrDefault(l => l.Name == currency)!;
        var currencyValue = _sdtLang.GetFactory().GetEnumerationLiteral(currencyLit);

        var amountSdt = _sdtLang.StructuredDataTypeByKey("key-SDTAmount");
        var amountValue = _sdtLang.GetFactory().CreateStructuredDataTypeInstance(amountSdt,
            new FieldValues
            {
                { amountSdt.FieldByKey("key-SDTValue"), decValue },
                { amountSdt.FieldByKey("key-SDTCurr"), currencyValue },
                { amountSdt.FieldByKey("key-SDTDigital"), digital }
            }
        );

        return amountValue;
    }
}