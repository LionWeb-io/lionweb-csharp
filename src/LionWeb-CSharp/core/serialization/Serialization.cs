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
public class SerializationChunk
{
    public string SerializationFormatVersion { get; set; }
    public SerializedLanguageReference[] Languages { get; set; }
    public SerializedNode[] Nodes { get; set; }
}

public class SerializedLanguageReference {
    public string Key { get; set; }
    public string Version { get; set; }
}

public class SerializedNode
{
    public string Id { get; set; }
    public MetaPointer Classifier { get; set; }
    public SerializedProperty[] Properties { get; set; }
    public SerializedContainment[] Containments { get; set; }
    public SerializedReference[] References { get; set; }
    public string[] Annotations { get; set; }
    public string? Parent { get; set; }
}

public class MetaPointer
{
    public string Language { get; set; }
    public string Version { get; set; }
    public string Key { get; set; }

    public override string ToString()
        => $"{Key} of ({Language}, {Version})";
}

public class SerializedProperty
{
    public MetaPointer Property { get; set; }
    public string Value { get; set; }
}

public class SerializedContainment
{
    public MetaPointer Containment { get; set; }
    public string[] Children { get; set; }
}

public class SerializedReferenceTarget {
    public string? ResolveInfo { get; set; }
    public string Reference { get; set; }
}

public class SerializedReference {
    public MetaPointer Reference { get; set; }
    public SerializedReferenceTarget[] Targets { get; set; }
}