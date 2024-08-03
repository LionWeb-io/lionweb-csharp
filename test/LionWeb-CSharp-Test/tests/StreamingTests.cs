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

namespace LionWeb.Core.Test;

using Examples.Shapes.M2;
using M1;
using M3;
using Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Stream;

[TestClass]
public class StreamingTests
{
    private readonly Language _language;

    public StreamingTests()
    {
        _language = ShapesLanguage.Instance;
    }

    const long maxSize = 1_500_000L;

    [TestMethod]
    public void MassSerialization()
    {
        using Stream stream = File.Create("output.json");
        JsonUtils.WriteNodesToStream(stream, new Serializer(), CreateNodes(maxSize));

        IEnumerable<INode> CreateNodes(long count)
        {
            Line? lastLine = null;
            Circle? lastCircle = null;
            Coord? lastCoord = null;
            for (long l = 0; l < count; l++)
            {
                var id = $"id{l}_{StringRandomizer.RandomLength()}";

                if (l % 10_000 == 0)
                {
                    TestContext.WriteLine(
                        $"Creating Line #{l} privateMem: {AsFraction(Process.GetCurrentProcess().PrivateMemorySize64)} gcMem: {AsFraction(GC.GetTotalMemory(false))}");
                }

                INode result;
                if (lastCoord == null)
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
                    result = new Geometry(id) { Shapes = [lastLine, lastCircle] };
                } else
                {
                    lastCoord = new Coord(id);
                    result = lastCoord;
                }

                yield return result;
            }
        }
    }

    string AsFraction(long value1)
    {
        return string.Format("{0:0.000}", value1 / 1_000_000D) + "M";
    }

    [TestMethod]
    public async Task MassDeserialization()
    {
        using Stream stream = File.OpenRead("output.json");

        var deserializer = new DeserializerBuilder()
            .WithLanguage(_language)
            .Build();

        long count = 0;

        async Task<List<INode>> doit(Stream stream, IDeserializer deserializer)
        {
            var streamReader = new Utf8JsonAsyncStreamReader(stream, leaveOpen: true);

            JsonSerializerOptions _readOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            bool insideNodes = false;
            while (await Advance())
            {
                switch (streamReader.TokenType)
                {
                    case JsonTokenType.PropertyName when streamReader.GetString() == "serializationFormatVersion":
                        await Advance();
                        string version = streamReader.GetString();
                        TestContext.WriteLine($"version: {version}");
                        break;

                    case JsonTokenType.PropertyName when streamReader.GetString() == "nodes":
                        insideNodes = true;
                        break;

                    case JsonTokenType.PropertyName when streamReader.GetString() != "nodes":
                        insideNodes = false;
                        break;

                    case JsonTokenType.StartObject when insideNodes:
                        try
                        {
                            var serializedNode = await streamReader.DeserializeAsync<SerializedNode>(_readOptions);
                            if (serializedNode != null)
                            {
                                await Task.Run(() =>
                                {
                                    deserializer.Process(serializedNode);
                                    if (count++ % 10_000 == 0)
                                    {
                                        TestContext.WriteLine(
                                            $"Parsing Entry #{count} privateMem: {AsFraction(Process.GetCurrentProcess().PrivateMemorySize64)} gcMem: {AsFraction(GC.GetTotalMemory(false))}");
                                    }
                                });
                            }
                        } catch { }
                        break;
                }
            }

            return deserializer.Finish().ToList();

            async Task<bool> Advance()
            {
                return await streamReader.ReadAsync();
            }
        }


        List<INode> nodes = await doit(stream, deserializer);

        Assert.AreEqual(maxSize, nodes.SelectMany(n => n.Descendants(true, true)).Count());
    }

    private TestContext testContextInstance;

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }
}

static class StringRandomizer
{
    // Constant seed for reproducible tests
    static Random random = new Random(0x1EE7);
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";

    public static string RandomLength() =>
        Random(random.Next(500));

    public static string Random(int length) =>
        new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
}