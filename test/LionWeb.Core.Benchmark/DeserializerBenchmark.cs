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

[MemoryDiagnoser]
// [NativeMemoryProfiler]
[TestClass]
[Ignore("must run after SerializerBenchmark")]
public class DeserializerBenchmark : SerializerBenchmarkBase
{
    [Benchmark]
    [TestMethod]
    public async Task Deserialize_Stream_Async()
    {
        await using Stream stream = File.OpenRead(_streamFile);

        IDeserializer deserializer = Deserializer();

        List<IReadableNode> nodes = await JsonUtils.ReadNodesFromStreamAsync(stream, deserializer, null, _simpleOptions);

        var actual = nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count();
        if (_maxSize != actual)
            throw new Exception($"Assertion failed: {actual} should be {_maxSize}");
    }

    [Benchmark]
    [TestMethod]
    public async Task Deserialize_Stream_Async_Aot()
    {
        await using Stream stream = File.OpenRead(_streamFile);

        IDeserializer deserializer = Deserializer();

        List<IReadableNode> nodes = await JsonUtils.ReadNodesFromStreamAsync(stream, deserializer, null, _aotOptions);

        var actual = nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count();
        if (_maxSize != actual)
            throw new Exception($"Assertion failed: {actual} should be {_maxSize}");
    }

    [Benchmark]
    [TestMethod]
    public void Deserialize_Stream()
    {
        using Stream stream = File.OpenRead(_streamFile);

        IDeserializer deserializer = Deserializer();

        var nodes = JsonUtils.ReadNodesFromStream(stream, deserializer, null, _simpleOptions);

        var actual = nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count();
        if (_maxSize != actual)
            throw new Exception($"Assertion failed: {actual} should be {_maxSize}");
    }

    [Benchmark]
    [TestMethod]
    public void Deserialize_Stream_Aot()
    {
        using Stream stream = File.OpenRead(_streamFile);

        IDeserializer deserializer = Deserializer();

        var nodes = JsonUtils.ReadNodesFromStream(stream, deserializer, null,  _aotOptions);

        var actual = nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count();
        if (_maxSize != actual)
            throw new Exception($"Assertion failed: {actual} should be {_maxSize}");
    }

    [Benchmark]
    [TestMethod]
    public void Deserialize_String()
    {
        var input = JsonSerializer.Deserialize<SerializationChunk>(File.ReadAllText(_stringFile), _simpleOptions)!;
        var deserializer = Deserializer();
        var nodes = deserializer.Deserialize(input);

        var actual = nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count();
        if (_maxSize != actual)
            throw new Exception($"Assertion failed: {actual} should be {_maxSize}");
    }

    [Benchmark]
    [TestMethod]
    public void Deserialize_String_Aot()
    {
        var input = JsonSerializer.Deserialize<SerializationChunk>(File.ReadAllText(_stringFile), _aotOptions)!;
        var deserializer = Deserializer();
        var nodes = deserializer.Deserialize(input);

        var actual = nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count();
        if (_maxSize != actual)
            throw new Exception($"Assertion failed: {actual} should be {_maxSize}");
    }

    private IDeserializer Deserializer()
    {
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        deserializer.RegisterInstantiatedLanguage(LionWebVersions.v2024_1.BuiltIns);
        deserializer.RegisterInstantiatedLanguage(_language);
        return deserializer;
    }
}