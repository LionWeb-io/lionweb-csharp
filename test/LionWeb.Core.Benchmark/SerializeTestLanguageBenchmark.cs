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

namespace LionWeb.Core.Benchmark;

using BenchmarkDotNet.Attributes;
using M1;
using Serialization;
using System.Text.Json;
using Test.Languages.Generated.V2024_1.TestLanguage;
using Test.Serialization;

[MemoryDiagnoser]
// [NativeMemoryProfiler]
[TestClass]
public class SerializeTestLanguageBenchmark : SerializerBenchmarkBase
{
    private IEnumerable<INode> _nodes;

    public SerializeTestLanguageBenchmark() : base(TestLanguageLanguage.Instance, LionWebVersions.Current)
    {
    }

    [IterationSetup]
    [TestInitialize]
    public void CreateNodes()
    {
        _nodes = CreateNodes(_maxSize);
    }

    [Benchmark]
    [TestMethod]
    public void AA_Serialize_Stream_Aot()
    {
        using Stream stream = File.Create(_streamFile);
        ISerializer serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        IEnumerable<IReadableNode> nodes = _nodes;
        object data = new LazySerializationChunk
        {
            SerializationFormatVersion = serializer.LionWebVersion.VersionString,
            Nodes = serializer.Serialize(nodes),
            Languages = serializer.UsedLanguages
        };
        JsonSerializer.Serialize(stream, data, _aotOptions);
    }

    [Benchmark]
    [TestMethod]
    public void BB_Deserialize_Stream_Aot()
    {
        using Stream stream = File.OpenRead(_streamFile);

        IDeserializer deserializer = Deserializer();

        var nodes = JsonUtils.ReadNodesFromStream(stream, deserializer, null, _aotOptions);

        var actual = nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count();
        if (_maxSize != actual)
            throw new Exception($"Assertion failed: {actual} should be {_maxSize}");
    }


    private static IEnumerable<INode> CreateNodes(long count)
    {
        LinkTestConcept? lastLine = null;
        LinkTestConcept? lastCircle = null;
        LinkTestConcept? lastCoord = null;
        for (long l = 0; l < count; l++)
        {
            var id = $"id{l}_{StringRandomizer.RandomLength()}";

            // if (l % 10_000 == 0)
            // {
            //     TestContext.WriteLine(
            //         $"Creating Line #{l} privateMem: {AsFraction(Process.GetCurrentProcess().PrivateMemorySize64)} gcMem: {AsFraction(GC.GetTotalMemory(false))}");
            // }

            INode result;
            if (lastCoord == null || l % 2 == 0)
            {
                lastCoord = new LinkTestConcept(id);
                result = lastCoord;
            } else if (l % 3 == 0)
            {
                lastLine = new LinkTestConcept(id) { Containment_0_1 = lastCoord };
                result = lastLine;
            } else if (l % 17 == 0)
            {
                lastCircle = new LinkTestConcept(id) { Reference_0_1 = lastCoord };
                result = lastCircle;
            } else if (l % 37 == 0)
            {
                result = new LinkTestConcept(id) { Reference_1_n = [lastLine!, lastCircle!] };
            } else
            {
                lastCoord = new LinkTestConcept(id);
                result = lastCoord;
            }

            yield return result;
        }
    }

    private IDeserializer Deserializer()
    {
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        deserializer.RegisterInstantiatedLanguage(_lionWebVersion.BuiltIns);
        deserializer.RegisterInstantiatedLanguage(_language);
        return deserializer;
    }
}