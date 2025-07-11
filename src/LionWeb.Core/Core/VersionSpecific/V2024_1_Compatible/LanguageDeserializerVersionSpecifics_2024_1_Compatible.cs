﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.VersionSpecific.V2024_1_Compatible;

using M2;
using M3;
using Serialization;
using V2024_1;

/// <see cref="LanguageDeserializer"/> parts specific to LionWeb <see cref="IVersion2024_1_Compatible"/>.  
internal class LanguageDeserializerVersionSpecifics_2024_1_Compatible(
    LanguageDeserializer deserializer,
    ILanguageDeserializerHandler handler)
    : LanguageDeserializerVersionSpecifics_2024_1(deserializer, handler)
{
    public override DynamicIKeyed CreateNodeWithProperties(SerializedNode serializedNode, NodeId id) =>
        new NodeCreator_2024_1_Compatible(this, serializedNode, id).Create();

    public override void InstallLanguageContainments(SerializedNode serializedNode, IReadableNode node,
        ILookup<MetaPointerKey, IKeyed> serializedContainmentsLookup) =>
        new ContainmentsInstaller_2024_1_Compatible(this, serializedNode, node, serializedContainmentsLookup).Install();

    public override void InstallLanguageReferences(SerializedNode serializedNode, IReadableNode node,
        ILookup<MetaPointerKey, IKeyed?> serializedReferencesLookup) =>
        new ReferencesInstaller_2024_1_Compatible(this, serializedNode, node, serializedReferencesLookup).Install();
}

internal class NodeCreator_2024_1_Compatible(
    LanguageDeserializerVersionSpecifics_2024_1 versionSpecifics,
    SerializedNode serializedNode,
    NodeId id)
    : NodeCreator_2024_1(versionSpecifics, serializedNode, id)
{
    protected override LionWebVersions Version => LionWebVersions.v2024_1_Compatible;
}

internal class ContainmentsInstaller_2024_1_Compatible(
    LanguageDeserializerVersionSpecificsBase versionSpecifics,
    SerializedNode serializedNode,
    IReadableNode node,
    ILookup<MetaPointerKey, IKeyed> serializedContainmentsLookup)
    : ContainmentsInstaller_2024_1(versionSpecifics, serializedNode, node, serializedContainmentsLookup)
{
    protected override LionWebVersions Version => LionWebVersions.v2024_1_Compatible;
}

internal class ReferencesInstaller_2024_1_Compatible(
    LanguageDeserializerVersionSpecificsBase versionSpecifics,
    SerializedNode serializedNode,
    IReadableNode node,
    ILookup<MetaPointerKey, IKeyed?> serializedReferencesLookup)
    : ReferencesInstaller_2024_1(versionSpecifics, serializedNode, node, serializedReferencesLookup)
{
    protected override LionWebVersions Version => LionWebVersions.v2024_1_Compatible;
}