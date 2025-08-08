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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialization;
using System.Text.Json;
using System.Text.Json.Stream;
using Utf8JsonStreamReader;

[MemoryDiagnoser]
// [NativeMemoryProfiler]
[TestClass]
public class DeserializerBenchmark : SerializerBenchmarkBase
{
    [Benchmark]
    [TestMethod]
    public async Task Deserialize_Stream_Async()
    {
        await using Stream stream = File.OpenRead(_streamFile);

        IDeserializer deserializer = Deserializer();

        List<IReadableNode> nodes = await ReadNodesFromStreamAsync(stream, deserializer, _simpleOptions);

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

        List<IReadableNode> nodes = await ReadNodesFromStreamAsync(stream, deserializer, _aotOptions);

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

        var nodes = ReadNodesFromStream(stream, deserializer,  _simpleOptions);

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

        var nodes = ReadNodesFromStream(stream, deserializer,  _aotOptions);

        var actual = nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count();
        if (_maxSize != actual)
            throw new Exception($"Assertion failed: {actual} should be {_maxSize}");
    }

    [Benchmark]
    [TestMethod]
    public void Deserialize_StreamX_Aot()
    {
        string.Intern("0");
        string.Intern("1");
        string.Intern("reference");
        string.Intern("annotations");
        string.Intern("children");
        string.Intern("classifier");
        string.Intern("containments");
        string.Intern("true");
        string.Intern("false");
        string.Intern("id");
        
        using Stream stream = File.OpenRead(_streamFile);

        IDeserializer deserializer = Deserializer();

        List<IReadableNode> ret;
        var bufferSize = 1024 * 8;
        
        using (var jsonStreamReader = new Utf8JsonStreamReaderX(stream, bufferSize))
        {
            var insideNodes = false;
            while (jsonStreamReader.Read())
            {
                
                switch (jsonStreamReader.TokenType)
                {
                    case JsonTokenType.PropertyName when jsonStreamReader.GetString() == "serializationFormatVersion":
                        jsonStreamReader.Read();
                        string? version = jsonStreamReader.GetString();
                        if (version != null)
                        {
                            if ((Action<string>?)null == null)
                            {
                                deserializer.LionWebVersion.AssureCompatible(version);
                            } else
                            {
                                ((Action<string>?)null)(version);
                            }
                        }
        
                        break;
        
                    case JsonTokenType.PropertyName when jsonStreamReader.GetString() == "nodes":
                        insideNodes = true;
                        break;
        
                    case JsonTokenType.PropertyName when jsonStreamReader.GetString() != "nodes":
                        insideNodes = false;
                        break;
        
                    case JsonTokenType.StartObject when insideNodes:
                        var serializedNode = jsonStreamReader.Deserialize<SerializedNode>(_aotOptions);
                        if (serializedNode != null)
                            deserializer.Process(serializedNode);
        
                        break;
                }
            }
        
            ret = deserializer.Finish().ToList();
        }

        var nodes = ret;

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

    private static async Task<List<IReadableNode>> ReadNodesFromStreamAsync(Stream utf8JsonStream,
        IDeserializer deserializer, JsonSerializerOptions jsonSerializerOptions, Action<string>? lionWebVersionChecker = null)
    {
        var streamReader = new Utf8JsonAsyncStreamReader(utf8JsonStream, leaveOpen: true);

        bool insideNodes = false;
        while (await Advance())
        {
            switch (streamReader.TokenType)
            {
                case JsonTokenType.PropertyName when streamReader.GetString() == "serializationFormatVersion":
                    await Advance();
                    string? version = streamReader.GetString();
                    if (version != null)
                    {
                        if (lionWebVersionChecker == null)
                        {
                            deserializer.LionWebVersion.AssureCompatible(version);
                        } else
                        {
                            lionWebVersionChecker(version);
                        }
                    }

                    break;

                case JsonTokenType.PropertyName when streamReader.GetString() == "nodes":
                    insideNodes = true;
                    break;

                case JsonTokenType.PropertyName when streamReader.GetString() != "nodes":
                    insideNodes = false;
                    break;

                case JsonTokenType.StartObject when insideNodes:
                    var serializedNode = await streamReader.DeserializeAsync<SerializedNode>(jsonSerializerOptions);
                    if (serializedNode != null)
                        deserializer.Process(serializedNode);

                    break;
            }
        }

        return deserializer.Finish().ToList();

        async Task<bool> Advance() => await streamReader.ReadAsync();
    }
    
    public static List<IReadableNode> ReadNodesFromStream(Stream utf8JsonStream, IDeserializer deserializer, JsonSerializerOptions jsonSerializerOptions,
        Action<string>? lionWebVersionChecker = null)
    {
        var bufferSize = 1024 * 8;
        
        using var jsonStreamReader = new Utf8JsonStreamReader(utf8JsonStream, bufferSize);
        
        var insideNodes = false;
        while (jsonStreamReader.Read())
        {
                
            switch (jsonStreamReader.TokenType)
            {
                case JsonTokenType.PropertyName when jsonStreamReader.GetString() == "serializationFormatVersion":
                    jsonStreamReader.Read();
                    string? version = jsonStreamReader.GetString();
                    if (version != null)
                    {
                        if (lionWebVersionChecker == null)
                        {
                            deserializer.LionWebVersion.AssureCompatible(version);
                        } else
                        {
                            lionWebVersionChecker(version);
                        }
                    }
        
                    break;
        
                case JsonTokenType.PropertyName when jsonStreamReader.GetString() == "nodes":
                    insideNodes = true;
                    break;
        
                case JsonTokenType.PropertyName when jsonStreamReader.GetString() != "nodes":
                    insideNodes = false;
                    break;
        
                case JsonTokenType.StartObject when insideNodes:
                    var serializedNode = jsonStreamReader.Deserialize<SerializedNode>(jsonSerializerOptions);
                    if (serializedNode != null)
                        deserializer.Process(serializedNode);
        
                    break;
            }
        }
        
        return deserializer.Finish().ToList();
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