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

namespace LionWeb.Core.Test.Serialization.Protobuf;

using Google.Protobuf;
using Io.Lionweb.Protobuf;
using LionWeb.Core.Test.Languages.Generated.V2023_1.Shapes.M2;
using LionWeb.Core.Utilities;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class Protobuf
{
    private readonly Geometry geo = new Geometry("g") { Documentation = new Documentation("d") { Text = "hello" } };

    [TestMethod]
    public void Write()
    {
        var serializer = new ProtobufSerializer([geo]);
        var pbChunk = serializer.Serialize();

        using var output = File.Create("chunk.protobuf");
        pbChunk.WriteTo(output);
    }

    [TestMethod]
    public void Read()
    {
        using var input = File.OpenRead("chunk.protobuf");
        var pbChunk = PBChunk.Parser.ParseFrom(input);

        var deserializer = new ProtobufDeserializer(pbChunk);
        var roots = deserializer.Deserialize([ShapesLanguage.Instance]).ToList();

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