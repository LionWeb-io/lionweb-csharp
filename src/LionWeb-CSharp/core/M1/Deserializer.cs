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

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;
using CompressedContainment = (CompressedMetaPointer, List<CompressedId>);
using CompressedReference = (CompressedMetaPointer, List<(CompressedId?, string?)>);

/// <summary>
/// Instances of this class can deserialize a <see cref="SerializationChunk"/> as a list of <see cref="IWritableNode"/>s that are root nodes.
/// An instance is parametrized with a collection of <see cref="Language"/> definitions with a corresponding <see cref="INodeFactory"/>.
/// </summary>
public partial class Deserializer : DeserializerBase<IWritableNode>
{
    private readonly Dictionary<CompressedId, List<CompressedContainment>> _containmentsByOwnerId = new();
    private readonly Dictionary<CompressedId, List<CompressedReference>> _referencesByOwnerId = new();
    private readonly Dictionary<CompressedId, List<CompressedId>> _annotationsByOwnerId = new();

    /// <summary>
    /// TODO
    /// </summary>
    public Deserializer()
    {
        RegisterInstantiatedLanguage(BuiltInsLanguage.Instance, BuiltInsLanguage.Instance.GetFactory());
    }
}