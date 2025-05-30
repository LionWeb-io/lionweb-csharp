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

namespace LionWeb.Core.Test.Serialization;

using Core.Serialization;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M3;
using System.Diagnostics;

[TestClass]
public class StreamingTests
{
    private readonly Language _language;
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    public StreamingTests()
    {
        _language = ShapesLanguage.Instance;
    }

    // private const long _maxSize = 1_500_000L;
    private const long _maxSize = 1_500L;

    [TestMethod]
    public void MassSerialization()
    {
        using Stream stream = File.Create("output.json");
        JsonUtils.WriteNodesToStream(stream, new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build(),
            CreateNodes(_maxSize));

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

    static string AsFraction(long value) =>
        $"{value / 1_000_000D:0.000}" + "M";


    [TestMethod]
    public void MassDeserialization()
    {
        using Stream stream =
            File.OpenRead("output.json");

        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        deserializer.RegisterInstantiatedLanguage(LionWebVersions.v2024_1.BuiltIns);
        deserializer.RegisterInstantiatedLanguage(_language);

        List<IReadableNode> nodes = JsonUtils.ReadNodesFromStreamAsync(stream, deserializer).GetAwaiter().GetResult();

        Assert.AreEqual(_maxSize, nodes.Cast<INode>().SelectMany(n => n.Descendants(true, true)).Count());
    }

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext { get; set; }
}

internal static class StringRandomizer
{
    // Constant seed for reproducible tests
    private static readonly Random _defaultRandom = new Random(0x1EE7);
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";

    public static string RandomLength() =>
        Random(_defaultRandom.Next(500));

    public static string Random(int length) =>
        new string(Enumerable.Repeat(_chars, length)
            .Select(s => s[_defaultRandom.Next(s.Length)]).ToArray());
}