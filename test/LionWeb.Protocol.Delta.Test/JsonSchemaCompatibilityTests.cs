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

namespace LionWeb.Protocol.Delta.Test;

using Json.Schema;
using System.Text.Json.Nodes;

[TestClass]
public class JsonSchemaCompatibilityTests : JsonTestsBase
{
    private static IEnumerable<object[]> GetTestData() => CollectAllMessages();
    
    private static JsonSchema JsonSchema { get; }

    static JsonSchemaCompatibilityTests()
    {
        using Stream? schemaStream =
            typeof(JsonSchemaCompatibilityTests).Assembly.GetManifestResourceStream("LionWeb.Protocol.Delta.Test.resources.delta.schema.json");
        Assert.IsNotNull(schemaStream);
        JsonSchema = JsonSchema.FromStream(schemaStream).Result;
    }

    [TestMethod]
    [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
    public void SchemaCompatibility(IDeltaContent delta)
    {
        var deltaSerializer = new DeltaSerializer();
        var serialized = deltaSerializer.Serialize(delta);

        var jsonNode = JsonNode.Parse(serialized);
        Assert.IsNotNull(jsonNode);
        EvaluationResults evaluationResults = JsonSchema.Evaluate(jsonNode, new EvaluationOptions()
        {
            OutputFormat = OutputFormat.List
        });
        Assert.IsTrue(evaluationResults.IsValid, serialized);
    }
}