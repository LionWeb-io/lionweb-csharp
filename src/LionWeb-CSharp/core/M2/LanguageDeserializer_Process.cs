// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.M2;

using M3;
using Serialization;

public partial class LanguageDeserializer
{
    /// <inheritdoc />
    public override void Process(SerializedNode serializedNode)
    {
        _serializedNodesById[Compress(serializedNode.Id)] = serializedNode;
        if (!IsLanguageNode(serializedNode))
            return;

        ProcessInternal(serializedNode, id => CreateNodeWithProperties(serializedNode, id));
    }

    private DynamicIKeyed CreateNodeWithProperties(SerializedNode serializedNode, string id) =>
        _languageVersionSpecifics.CreateNodeWithProperties(serializedNode, id);
}