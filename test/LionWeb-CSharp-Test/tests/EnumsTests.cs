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

namespace LionWeb.Enums.Test;

using Core.M1;
using Core.M2;
using Core.M3;
using Core.Serialization;
using Examples.WithEnum.M2;

[TestClass]
public class EnumsTests
{
    private readonly Language _language;
    private readonly WithEnumFactory _factory;
    private readonly LionWebVersions _lionWebVersions = LionWebVersions.Current;

    public EnumsTests()
    {
        _language = WithEnumLanguage.Instance;
        _factory = _language.GetFactory() as WithEnumFactory;
    }

    [TestMethod]
    public void LionCoreKeysOnEnums()
    {
        Assert.AreEqual("lit1", MyEnum.literal1.LionCoreKey());
        Assert.AreEqual(MyEnum.literal2, _factory.GetEnumerationLiteral(_language.Enumerations().First().Literals[1]));
    }

    private EnumHolder Model()
    {
        var holder = _factory.CreateEnumHolder();
        holder.EnumValue = MyEnum.literal1;
        return holder;
    }

    [TestMethod]
    public void RoundtripSerializationOfModelWithEnum()
    {
        var nodes = new[] { Model() };
        var chunk = new Serializer(_lionWebVersions).SerializeToChunk(nodes);
        Console.WriteLine(JsonUtils.WriteJsonToString(chunk));
        Assert.AreEqual("lit1", chunk.Nodes[0].Properties[0].Value);

        var deserialization = new DeserializerBuilder()
            .WithLanguage(_language)
            .Build()
            .Deserialize(chunk);
        Assert.AreEqual(MyEnum.literal1, (deserialization[0] as EnumHolder).EnumValue);

        var reserialization = new Serializer(_lionWebVersions).SerializeToChunk(deserialization);
        Assert.AreEqual(JsonUtils.WriteJsonToString(chunk), JsonUtils.WriteJsonToString(reserialization));
    }
}