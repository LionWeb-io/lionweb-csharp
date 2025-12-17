// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test;

using Core.Serialization;
using Core.Utilities;
using M1;
using M2;
using @namespace.@int.@public.V2024_1;

[TestClass]
public class KeywordLangTests
{
    [TestMethod]
    public void Language()
    {
        LionWebVersions lionWebVersion = LionWebVersions.v2024_1;

        List<IReadableNode?> input = [ClassLanguage.Instance];
        SerializationChunk chunk = new SerializerBuilder().WithLionWebVersion(lionWebVersion).Build()
            .SerializeToChunk(input);

        List<IReadableNode?> deserialized = new LanguageDeserializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .Build()
            .Deserialize(chunk)
            .Cast<IReadableNode?>()
            .ToList();

        List<IDifference> differences = new Comparer(input, deserialized).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }

    [TestMethod]
    public void Instance()
    {
        LionWebVersions lionWebVersion = LionWebVersions.v2024_1;

        @out root = new @out("out")
        {
            Ref = new record("record")
            {
                String = @enum.@internal, Double = new @struct("struct") { Ref = new @var("var") }
            },
            Default = new @if { Namespace = "hello"}
        };

        List<IReadableNode?> input = [root, root.Ref, ((@struct)root.Ref.Double).Ref];
        SerializationChunk chunk = new SerializerBuilder().WithLionWebVersion(lionWebVersion).Build()
            .SerializeToChunk(input);

        IDeserializer deserializer = new DeserializerBuilder()
            .WithCompressedIds(new(KeepOriginal: true))
            .WithLanguage(ClassLanguage.Instance)
            .Build();

        List<IReadableNode?> deserialized = deserializer
            .Deserialize(chunk)
            .Cast<IReadableNode?>()
            .ToList();

        List<IDifference> differences = new Comparer(input, deserialized).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
}