// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Serialization;

using Core.Serialization;
using Languages.Generated.V2024_1.Mixed.MixedConceptLang;
using Languages.Generated.V2024_1.Mixed.MixedDirectEnumLang;
using Languages.Generated.V2024_1.Mixed.MixedDirectSdtLang;
using Languages.Generated.V2024_1.Mixed.MixedNestedEnumLang;
using Languages.Generated.V2024_1.Mixed.MixedNestedSdtLang;
using Languages.Generated.V2024_1.SDTLang;
using M1;

[TestClass]
public class SerializationPropertyTests : SerializationTestsBase
{
    [TestMethod]
    public void SerializeStructuredDataType()
    {
        var node = new SDTConcept("nodeId")
        {
            Amount =
                new Amount { Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true },
            Decimal = new Decimal { Int = 19 },
            Complex = new ComplexNumber
            {
                Real = new Decimal { Int = 1, Frac = 0 },
                Imaginary = new Languages.Generated.V2024_1.SDTLang.Decimal()
            }
        };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var serializedNodes = serializer.Serialize([node]).ToList();
        Assert.AreEqual(1, serializedNodes.Count);
        var serializedNode = serializedNodes.First();

        Assert.AreEqual(
            """{"key-SDTCurr":"key-SDTEur","key-SDTDigital":"true","key-SDTValue":{"key-SDTFrac":"42","key-SDTInt":"23"}}""",
            serializedNode.Properties.First(p => p.Property.Key == "key-SDTamountField").Value);

        Assert.AreEqual(
            """{"key-SDTInt":"19"}""",
            serializedNode.Properties.First(p => p.Property.Key == "key-SDTDecimalField").Value);

        Assert.AreEqual(
            """{"key-SDTImaginary":{},"key-SDTReal":{"key-SDTFrac":"0","key-SDTInt":"1"}}""",
            serializedNode.Properties.First(p => p.Property.Key == "key-SDTComplexField").Value);


        var nodes = new DeserializerBuilder()
            .WithLanguage(SDTLangLanguage.Instance)
            .WithLanguage(SDTLangLanguage.Instance)
            .Build()
            .Deserialize(serializedNodes);

        AssertEquals([node], nodes);
    }

    [TestMethod]
    public void SerializePropertyUsedLanguages()
    {
        var node = new MixedConcept("mixedId")
        {
            EnumProp = DirectEnum.directEnumA,
            SdtProp = new DirectSdt
            {
                DirectSdtEnum = NestedEnum.nestedLiteralA,
                DirectSdtSdt = new NestedSdt { NestedSdtField = "hello" }
            }
        };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var serializedNodes = serializer.Serialize([node]).ToList();
        Assert.AreEqual(1, serializedNodes.Count);
        CollectionAssert.AreEquivalent(new List<SerializedLanguageReference>
        {
            new() { Key = "key-mixedBasePropertyLang", Version = "1" },
            new() { Key = "key-mixedBaseContainmentLang", Version = "1" },
            new() { Key = "key-mixedBaseReferenceLang", Version = "1" },
            new() { Key = "key-mixedBaseConceptLang", Version = "1" },
            new() { Key = "key-mixedConceptLang", Version = "1" },
            new() { Key = "key-mixedDirectEnumLang", Version = "1" },
            new() { Key = "key-mixedNestedEnumLang", Version = "1" },
            new() { Key = "key-mixedDirectSdtLang", Version = "1" },
            new() { Key = "key-mixedNestedSdtLang", Version = "1" },
        }, serializer.UsedLanguages.ToList());
    }
}