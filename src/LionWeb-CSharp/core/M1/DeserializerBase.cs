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

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;

public abstract class DeserializerBase<TVersion, TBuiltIns, TM3> : IDeserializer, ILionWebVersionUser<TVersion, TBuiltIns, TM3>
where TVersion: LionWebVersions
where TBuiltIns : IBuiltInsLanguage
where TM3 : ILionCoreLanguage
{
    protected readonly ILionCoreLanguage _m3;
    protected readonly IBuiltInsLanguage _builtIns;

    protected readonly DeserializerMetaInfo _deserializerMetaInfo = new();
    protected readonly Dictionary<CompressedId, IReadableNode> _dependentNodesById = new();
    
    protected DeserializerBase(TVersion lionWebVersion)
    {
        LionWebVersion = lionWebVersion;
        _m3 = lionWebVersion.LionCore;
        _builtIns = lionWebVersion.BuiltIns;
    }

    TBuiltIns ILionWebVersionUser<TVersion, TBuiltIns, TM3>.UBuiltIns => (TBuiltIns)_builtIns;
    TM3 ILionWebVersionUser<TVersion, TBuiltIns, TM3>.UM3 => (TM3)_m3;
    TVersion ILionWebVersionUser<TVersion, TBuiltIns, TM3>.ULionWebVersion => (TVersion)LionWebVersion;

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get; }

    /// <inheritdoc />
    public IDeserializerHandler Handler
    {
        get => _deserializerMetaInfo.Handler;
        init => _deserializerMetaInfo.Handler = value;
    }

    /// <summary>
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
        => _deserializerMetaInfo.RegisterInstantiatedLanguage(language, factory);

    /// <inheritdoc />
    public abstract void Process(SerializedNode serializedNode);

    /// <inheritdoc />
    public abstract IEnumerable<IReadableNode> Finish();

    
    protected internal CompressedId Compress(string id) =>
        CompressedId.Create(id, StoreUncompressedIds);

    protected CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, StoreUncompressedIds);

    protected bool IsInDependentNodes(CompressedId compressedId) =>
        _dependentNodesById.ContainsKey(compressedId);
}

public abstract class DeserializerBase<T, TVersion, TBuiltIns, TM3>(TVersion lionWebVersion)
    : DeserializerBase<TVersion, TBuiltIns, TM3>(lionWebVersion), IDeserializer<T>
    where T : IReadableNode
    where TVersion: LionWebVersions
    where TBuiltIns : IBuiltInsLanguage
    where TM3 : ILionCoreLanguage
{
    protected readonly Dictionary<CompressedId, T> _deserializedNodesById = new();

    /// <inheritdoc />
    public abstract override void Process(SerializedNode serializedNode);

    IEnumerable<T> IDeserializer<T>.Finish() => (IEnumerable<T>)Finish();

    /// <inheritdoc />
    public abstract override IEnumerable<IReadableNode> Finish();
}