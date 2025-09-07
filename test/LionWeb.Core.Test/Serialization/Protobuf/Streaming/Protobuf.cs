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

namespace LionWeb.Core.Test.Serialization.Protobuf.Streaming;

using Core.Utilities;
using Google.Protobuf;
using Io.Lionweb.Protobuf.Streaming;
using Languages.Generated.V2023_1.Shapes.M2;
using M1;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class Protobuf
{
    private readonly Geometry geo = new Geometry("g") { Documentation = new Documentation("d") { Text = "hello" } };

    [TestMethod]
    public void WriteBlock()
    {
        var serializer = new ProtobufBlockSerializer();
        var psChunk = serializer.Serialize([geo]);

        using var output = File.Create("chunkStreamingBlock.protobuf");
        psChunk.WriteTo(output);
    }

    [TestMethod]
    public void ReadBlock()
    {
        using var input = File.OpenRead("chunkStreamingBlock.protobuf");
        var psChunk = PsChunk.Parser.ParseFrom(input);

        var deserializer = new ProtobufBlockDeserializer([ShapesLanguage.Instance]);
        var roots = deserializer.Deserialize(psChunk).ToList();

        AssertEquals([geo], roots);
    }

    [TestMethod]
    public void WriteStream()
    {
        using var output = File.Create("chunkStreamingStream.protobuf");
        var serializer = new ProtobufStreamingSerializer(output);
        serializer.Serialize(geo.Descendants(true, true));
    }

    [TestMethod]
    public void ReadStream()
    {
        using var input = File.OpenRead("chunkStreamingStream.protobuf");

        var deserializer = new ProtobufStreamingDeserializer(input, [ShapesLanguage.Instance]);
        var roots = deserializer.Deserialize().ToList();

        AssertEquals([geo], roots);
    }

    protected void AssertEquals(INode? a, INode? b) =>
        AssertEquals([a], [b]);

    protected void AssertEquals(IEnumerable<INode?> a, IEnumerable<INode?> b)
    {
        List<IDifference> differences = new Comparer(a.ToList(), b.ToList()).Compare().ToList();
        Assert.IsTrue(differences.Count == 0,
            differences.DescribeAll(new() { LeftDescription = "a", RightDescription = "b" }));
    }
}