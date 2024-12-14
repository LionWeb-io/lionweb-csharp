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

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;

/// <inheritdoc />
public abstract class DeserializerBase<T> : IDeserializer<T> where T : IReadableNode
{
    protected readonly IDeserializerVersionSpecifics VersionSpecifics;
    protected readonly ILionCoreLanguage _m3;

    /// LionCore builtins according to <see cref="_versionSpecifics"/>.
    protected readonly IBuiltInsLanguage _builtIns;

    /// <inheritdoc cref="DeserializerMetaInfo"/>
    protected readonly DeserializerMetaInfo _deserializerMetaInfo = new();

    /// <inheritdoc cref="IDeserializer.RegisterDependentNodes"/>
    protected readonly Dictionary<CompressedId, IReadableNode> _dependentNodesById = new();

    /// Already deserialized nodes.
    protected readonly Dictionary<CompressedId, T> _deserializedNodesById = new();

    /// <param name="versionSpecifics">Version of LionWeb standard to use.</param>
    protected DeserializerBase(IDeserializerVersionSpecifics versionSpecifics)
    {
        VersionSpecifics = versionSpecifics;
        _m3 = versionSpecifics.Version.LionCore;
        _builtIns = versionSpecifics.Version.BuiltIns;
    }

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get => VersionSpecifics.Version; }

    /// <inheritdoc />
    public IDeserializerHandler Handler
    {
        get => _deserializerMetaInfo.Handler;
        init => _deserializerMetaInfo.Handler = value;
    }

    /// Whether we store uncompressed <see cref="IReadableNode.GetId()">node ids</see> and <see cref="MetaPointer">MetaPointers</see> during deserialization.
    /// Uses more memory, but very helpful for debugging. 
    /// </summary>
    public bool StoreUncompressedIds
    {
        get => _deserializerMetaInfo.StoreUncompressedIds;
        init => _deserializerMetaInfo.StoreUncompressedIds = value;
    }

    /// <inheritdoc />
    public virtual void RegisterDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        foreach (var dependentNode in dependentNodes)
        {
            _dependentNodesById[Compress(dependentNode.GetId())] = dependentNode;
        }
    }

    /// <inheritdoc />
    public virtual void RegisterInstantiatedLanguage(Language language, INodeFactory factory)
    {
        if(!language.LionWebVersion.Equals(LionWebVersion))
            throw new VersionMismatchException(language.LionWebVersion.VersionString, LionWebVersion.VersionString, $"[{language.Key}, {language.Version}]");
            
        _deserializerMetaInfo.RegisterInstantiatedLanguage(language, factory);
    }

    /// <inheritdoc />
    public abstract void Process(SerializedNode serializedNode);

    /// <inheritdoc />
    public abstract IEnumerable<T> Finish();

    
    protected internal CompressedId Compress(string id) =>
        CompressedId.Create(id, StoreUncompressedIds);

    protected CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, StoreUncompressedIds);

    /// Checks whether <paramref name="compressedId"/> is contained in <see cref="_dependentNodesById"/>.
    protected bool IsInDependentNodes(CompressedId compressedId) =>
        _dependentNodesById.ContainsKey(compressedId);
}