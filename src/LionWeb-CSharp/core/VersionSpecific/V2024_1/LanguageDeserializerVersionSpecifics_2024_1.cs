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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.VersionSpecific.V2024_1;

using M1;
using M2;
using M3;
using Serialization;

/// <see cref="LanguageDeserializer"/> parts specific to LionWeb <see cref="IVersion2024_1"/>.  
internal class LanguageDeserializerVersionSpecifics_2024_1(
    LanguageDeserializer deserializer,
    ILanguageDeserializerHandler handler)
    : LanguageDeserializerVersionSpecificsBase(deserializer, handler)
{
    public override LionWebVersions Version => LionWebVersions.v2024_1;

    public override DynamicIKeyed CreateNodeWithProperties(SerializedNode serializedNode, string id) =>
        new NodeCreator_2024_1(this, serializedNode, id).Create();

    public override void InstallLanguageContainments(SerializedNode serializedNode, IReadableNode node,
        ILookup<string, IKeyed> serializedContainmentsLookup) =>
        new ContainmentsInstaller_2024_1(this, serializedNode, node, serializedContainmentsLookup).Install();

    public override void InstallLanguageReferences(SerializedNode serializedNode, IReadableNode node,
        ILookup<string, IKeyed?> serializedReferencesLookup) =>
        new ReferencesInstaller_2024_1(this, serializedNode, node, serializedReferencesLookup).Install();
}

internal class NodeCreator_2024_1(
    LanguageDeserializerVersionSpecifics_2024_1 versionSpecifics,
    SerializedNode serializedNode,
    string id)
    : NodeCreatorBase(versionSpecifics, serializedNode, id)
{
    protected override LionWebVersions Version => LionWebVersions.v2024_1;
    protected override ILionCoreLanguageWithStructuredDataType LionCore => (ILionCoreLanguage_2024_1)Version.LionCore;

    public override DynamicIKeyed Create() => _serializedNode.Classifier switch
    {
        var s when s.Key == LionCore.StructuredDataType.Key => new DynamicStructuredDataType(_id, null)
        {
            Key = _key, Name = _name
        },
        var s when s.Key == LionCore.Field.Key => new DynamicField(_id, null) { Key = _key, Name = _name },
        _ => base.Create()
    };
}

internal class ContainmentsInstaller_2024_1(
    LanguageDeserializerVersionSpecificsBase versionSpecifics,
    SerializedNode serializedNode,
    IReadableNode node,
    ILookup<string, IKeyed> serializedContainmentsLookup)
    : ContainmentsInstallerBase(versionSpecifics, serializedNode, node, serializedContainmentsLookup)
{
    protected override LionWebVersions Version => LionWebVersions.v2024_1;
    protected override ILionCoreLanguageWithStructuredDataType LionCore => (ILionCoreLanguage_2024_1)Version.LionCore;

    public override void Install()
    {
        switch (_node)
        {
            case DynamicStructuredDataType sdt:
                sdt.AddFields(Lookup<Field>(LionCore.StructuredDataType_fields));
                return;
            case DynamicField:
                return;
            default:
                base.Install();
                return;
        }
    }
}

internal class ReferencesInstaller_2024_1(
    LanguageDeserializerVersionSpecificsBase versionSpecifics,
    SerializedNode serializedNode,
    IReadableNode node,
    ILookup<string, IKeyed?> serializedReferencesLookup)
    : ReferencesInstallerBase(versionSpecifics, serializedNode, node, serializedReferencesLookup)
{
    protected override LionWebVersions Version => LionWebVersions.v2024_1;
    protected override ILionCoreLanguageWithStructuredDataType LionCore => (ILionCoreLanguage_2024_1)Version.LionCore;

    public override void Install()
    {
        switch (_node)
        {
            case DynamicField field:
                field.Type = LookupSingle<Datatype>(LionCore.Field_type)!;
                return;

            case StructuredDataType:
                return;

            default:
                base.Install();
                return;
        }
    }
}