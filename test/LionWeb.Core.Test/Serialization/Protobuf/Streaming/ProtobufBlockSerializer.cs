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

using Core.Serialization;
using Io.Lionweb.Protobuf.Streaming;
using M1;
using M2;
using M3;
using System.Collections;
using StringIndex = ulong;
using LanguageIndex = ulong;
using MetaPointerIndex = ulong;
using NodeIndex = ulong;

public class ProtobufBlockSerializer : ProtobufSerializerBase
{
    private readonly PsChunk _chunk;

    public ProtobufBlockSerializer()
    {
        var x = new PsSerializationFormatVersion { Version = _lionWebVersion.VersionString };
        _chunk = new PsChunk { SerializationFormatVersion = x };
    }

    public PsChunk Serialize(IEnumerable<IReadableNode> nodes)
    {
        nodes
            .SelectMany(n => M1Extensions.Descendants(n, true, true))
            .Select(Process)
            .ToList();

        return Finish();
    }

    public override PsNode Process(IReadableNode node)
    {
        var result = CreateNode(node);
        _chunk.Nodes.Add(result);
        return result;
    }

    public PsChunk Finish() => _chunk;

    protected override void AddString(PsStringValue stringValue) =>
        _chunk.StringValues.Add(stringValue);

    protected override void AddMetaPointer(PsMetaPointer metaPointer) =>
        _chunk.MetaPointers.Add(metaPointer);

    protected override void AddUsedLanguage(PsLanguage language) =>
        _chunk.Languages.Add(language);
}