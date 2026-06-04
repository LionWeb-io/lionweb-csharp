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

namespace LionWeb.Core.Benchmark;

using BenchmarkDotNet.Attributes;
using M1;
using Serialization;
using System.Text.Json;
using Test.Languages.Generated.V2024_1.TestLanguage;
using Test.Serialization;

[MemoryDiagnoser]
// [NativeMemoryProfiler]
public class SerializerBenchmark : SerializerBenchmarkBase
{
    private IEnumerable<INode> _nodes;

    [IterationSetup]
    [TestInitialize]
    public void CreateNodes()
    {
        _nodes = CreateNodes(_maxSize);
    }

    [Benchmark]
    public async Task Serialize_Stream_Async()
    {
        await using Stream stream = File.Create(_streamFile);
        ISerializer serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        IEnumerable<IReadableNode> nodes = _nodes;
        object data = new LazySerializationChunk
        {
            SerializationFormatVersion = serializer.LionWebVersion.VersionString,
            Nodes = serializer.Serialize(nodes),
            Languages = serializer.UsedLanguages
        };
        await JsonSerializer.SerializeAsync(stream, data, _simpleOptions);
    }

    [Benchmark]
    public void Serialize_Stream()
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
        JsonSerializer.Serialize(stream, data, _simpleOptions);
    }

    [Benchmark]
    public async Task Serialize_Stream_Async_Aot()
    {
        await using Stream stream = File.Create(_streamFile);
        ISerializer serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        IEnumerable<IReadableNode> nodes = _nodes;
        object data = new LazySerializationChunk
        {
            SerializationFormatVersion = serializer.LionWebVersion.VersionString,
            Nodes = serializer.Serialize(nodes),
            Languages = serializer.UsedLanguages
        };
        await JsonSerializer.SerializeAsync(stream, data, _aotOptions);
    }

    [Benchmark]
    public void Serialize_Stream_Aot()
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
    public void Serialize_String()
    {
        var output = JsonSerializer.Serialize((object)new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build()
            .SerializeToChunk(_nodes), _simpleOptions);

        File.WriteAllText(_stringFile, output);
    }

    [Benchmark]
    public void Serialize_String_Aot()
    {
        var output = JsonSerializer.Serialize((object)new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build()
            .SerializeToChunk(_nodes), _aotOptions);

        File.WriteAllText(_stringFile, output);
    }

    public static IEnumerable<INode> CreateNodes(long count)
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
                lastCircle = new LinkTestConcept(id) { Containment_1 = lastCoord };
                result = lastCircle;
            } else if (l % 37 == 0)
            {
                result = new LinkTestConcept(id) { Containment_0_n = [lastLine!, lastCircle!] };
            } else
            {
                lastCoord = new LinkTestConcept(id);
                result = lastCoord;
            }

            yield return result;
        }
    }
}