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
using Test.Languages.Generated.V2024_1.Shapes.M2;
using Test.Utilities;

[MemoryDiagnoser]
// [NativeMemoryProfiler]
[TestClass]
public class SerializerBenchmark : SerializerBenchmarkBase
{
    private IEnumerable<INode> _nodes;

    [IterationSetup]
    public void CreateNodes()
    {
        _nodes = CreateNodes(_maxSize);
    }

    [Benchmark]
    [TestMethod]
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
    [TestMethod]
    public void Serialize_Stream_Context()
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
    public void Serialize_String()
    {
        var output = JsonSerializer.Serialize((object)new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build()
            .SerializeToChunk(_nodes), _simpleOptions);
        
        File.WriteAllText(_stringFile, output);
    }

    [Benchmark]
    [TestMethod]
    public void Serialize_String_Context()
    {
        var output = JsonSerializer.Serialize((object)new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build()
            .SerializeToChunk(_nodes), _aotOptions);
        
        File.WriteAllText(_stringFile, output);
    }

    private static IEnumerable<INode> CreateNodes(long count)
    {
        Line? lastLine = null;
        Circle? lastCircle = null;
        Coord? lastCoord = null;
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
                lastCoord = new Coord(id);
                result = lastCoord;
            } else if (l % 3 == 0)
            {
                lastLine = new Line(id) { Start = lastCoord };
                result = lastLine;
            } else if (l % 17 == 0)
            {
                lastCircle = new Circle(id) { Center = lastCoord };
                result = lastCircle;
            } else if (l % 37 == 0)
            {
                result = new Geometry(id) { Shapes = [lastLine!, lastCircle!] };
            } else
            {
                lastCoord = new Coord(id);
                result = lastCoord;
            }

            yield return result;
        }
    }
}