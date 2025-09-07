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
using Core.Utilities;
using Google.Protobuf;
using Io.Lionweb.Protobuf;
using Io.Lionweb.Protobuf.Streaming;
using M1;
using M2;
using M3;
using System.Diagnostics;
using System.Net.Sockets;
using StringIndex = ulong;
using LanguageIndex = ulong;
using MetaPointerIndex = ulong;
using NodeIndex = ulong;
using ResolveInfo = string;
using CompressedContainment = (ulong, List<ulong>);
using CompressedReference = (ulong, List<(ulong?, ulong?)>);

public class ProtobufStreamingDeserializer : ProtobufDeserializerBase
{
    private Stream _stream;
    
    public ProtobufStreamingDeserializer(Stream input, IEnumerable<Language> languages) : base(languages)
    {
        _stream = input;
    }

    public IEnumerable<INode> Deserialize()
    {
        while (_stream.Position < _stream.Length)
        {
            var msg = PsMessage.Parser.ParseDelimitedFrom(_stream);
            // Console.WriteLine($"Reading {msg}");
            if (msg == null)
                break;
            Process(msg);
        }

        return Finish();
    }
}