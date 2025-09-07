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
using Google.Protobuf;
using Io.Lionweb.Protobuf.Streaming;
using M1;
using M2;
using M3;
using System.Collections;
using StringIndex = ulong;
using LanguageIndex = ulong;
using MetaPointerIndex = ulong;
using NodeIndex = ulong;

public class ProtobufStreamingSerializer : ProtobufSerializerBase
{
    private readonly Stream _stream;

    public ProtobufStreamingSerializer(Stream stream)
    {
        _stream = stream;
    }

    public void Serialize(IEnumerable<IReadableNode> nodes)
    {
        var version = new PsSerializationFormatVersion { Version = _lionWebVersion.VersionString };
        Write(new PsMessage { SerializationFormatVersion = version });
        
        foreach (var node in nodes)
        {
            Write(new PsMessage {Node = CreateNode(node)});
        }
    }

    private void Write(PsMessage msg)
    {
        // Console.WriteLine($"Writing {msg}");
        msg.WriteDelimitedTo(_stream);
    }

    public override PsNode Process(IReadableNode node) => CreateNode(node);

    protected override void AddString(PsStringValue stringValue) =>
        Write(new PsMessage {StringValue = stringValue});

    protected override void AddMetaPointer(PsMetaPointer metaPointer) =>
        Write(new PsMessage {MetaPointer = metaPointer});

    protected override void AddUsedLanguage(PsLanguage language) =>
        Write(new PsMessage{Language = language});
}