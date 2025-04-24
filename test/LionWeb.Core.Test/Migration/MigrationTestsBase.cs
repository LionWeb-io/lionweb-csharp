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

namespace LionWeb.Core.Test.Migration;

using Core.Migration;
using Core.Utilities;
using Core.Serialization;
using Languages.Generated.V2023_1.Shapes.M2;
using M1;
using M3;

public abstract class MigrationTestsBase
{
    protected static async Task<MemoryStream> Serialize(IReadableNode input) =>
        await Serialize([input]);

    protected static async Task<MemoryStream> Serialize(IEnumerable<IReadableNode> inputs)
    {
        var inputStream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(inputStream,
            new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1).Build(), inputs);
        inputStream.Seek(0, SeekOrigin.Begin);
        return inputStream;
    }

    protected static async Task<List<IReadableNode>> Deserialize(MemoryStream outputStream)
    {
        outputStream.Seek(0, SeekOrigin.Begin);
        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1).WithLanguage(ShapesLanguage.Instance)
                .Build());
        return resultNodes;
    }

    protected DynamicLanguage DynamicClone(LionWebVersions lionWebVersion, Language language) =>
        new DynamicLanguageCloner(lionWebVersion).Clone([language]).Values.First();

    protected static bool Compare(IReadableNode? expected, IReadableNode actual) =>
        new Comparer([expected], [actual]) { BehaviorConfig = new() { CompareCompatibleClassifier = false } }.AreEqual();

    protected static bool Compare(List<IReadableNode>? expected, List<IReadableNode> actual) =>
        new Comparer(expected ?? [], actual) { BehaviorConfig = new() { CompareCompatibleClassifier = false } }.AreEqual();
}