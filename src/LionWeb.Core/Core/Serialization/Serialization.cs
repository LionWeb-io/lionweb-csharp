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

namespace LionWeb.Core.Serialization;

/// <summary>
/// This type, together with all the types in this file, represent data structures
/// to capture a parsing of a LionWeb serialization chunk in JSON format.
/// </summary>
public record SerializationChunk
{
    public required string SerializationFormatVersion { get; init; }
    public required SerializedLanguageReference[] Languages { get; init; }
    public required SerializedNode[] Nodes { get; init; }
}

public record SerializedLanguageReference
{
    public required string Key { get; set; }
    public required string Version { get; set; }
}

public record SerializedNode
{
    public required string Id { get; init; }
    public required MetaPointer Classifier { get; init; }
    public required SerializedProperty[] Properties { get; init; }
    public required SerializedContainment[] Containments { get; init; }
    public required SerializedReference[] References { get; init; }
    public required string[] Annotations { get; init; }
    public string? Parent { get; init; }
}

public record MetaPointer(string Language, string Version, string Key);

public record SerializedProperty
{
    public required MetaPointer Property { get; init; }
    public string? Value { get; init; }
}

public record SerializedContainment
{
    public required MetaPointer Containment { get; init; }
    public required string[] Children { get; init; }
}

public record SerializedReferenceTarget
{
    public string? ResolveInfo { get; init; }
    public string? Reference { get; init; }
}

public record SerializedReference
{
    public required MetaPointer Reference { get; init; }
    public required SerializedReferenceTarget[] Targets { get; init; }
}