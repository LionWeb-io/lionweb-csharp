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
using CompressedReference = (CompressedMetaPointer, List<(CompressedId?, string?)>);

/// <inheritdoc />
public abstract class DeserializerBase<T> : IDeserializer<T> where T : class, IReadableNode
{
    /// Version of LionWeb standard to use.
    protected readonly IDeserializerVersionSpecifics _versionSpecifics;

    /// LionCore M3 according to <see cref="_versionSpecifics"/>.
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
        _versionSpecifics = versionSpecifics;
        _m3 = versionSpecifics.Version.LionCore;
        _builtIns = versionSpecifics.Version.BuiltIns;
        _versionSpecifics.Initialize(this, _deserializerMetaInfo, Handler);
    }

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get => _versionSpecifics.Version; }

    /// <inheritdoc />
    public IDeserializerHandler Handler
    {
        get => _deserializerMetaInfo.Handler;
        init
        {
            _deserializerMetaInfo.Handler = value;
            _versionSpecifics.Initialize(this, _deserializerMetaInfo, value);
        }
    }

    /// Whether we store uncompressed <see cref="IReadableNode.GetId()">node ids</see> and <see cref="MetaPointer">MetaPointers</see> during deserialization.
    /// Uses more memory, but very helpful for debugging. 
    public bool StoreUncompressedIds
    {
        get => _deserializerMetaInfo.StoreUncompressedIds;
        init => _deserializerMetaInfo.StoreUncompressedIds = value;
    }
    
    /// Whether we try to resolve references by <see cref="LionWeb.Core.Serialization.SerializedReferenceTarget.ResolveInfo"/>.
    public ReferenceResolveInfoHandling ResolveInfoHandling { get; init; }

    /// <inheritdoc />
    public virtual void RegisterDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        foreach (var dependentNode in dependentNodes)
        {
            _dependentNodesById[Compress(dependentNode.GetId())] = dependentNode;
        }
    }

    /// <inheritdoc />
    public virtual void RegisterInstantiatedLanguage(Language language, INodeFactory? factory = null)
    {
        LionWebVersion.AssureCompatible(language);
        _deserializerMetaInfo.RegisterInstantiatedLanguage(language, factory ?? language.GetFactory());
    }

    /// <inheritdoc />
    public abstract void Process(SerializedNode serializedNode);

    /// <inheritdoc />
    public abstract IEnumerable<T> Finish();

    /// Takes care of <see cref="IDeserializerHandler.SkipDeserializingDependentNode"/>
    /// and <see cref="IDeserializerHandler.DuplicateNodeId"/>.
    protected CompressedId? ProcessInternal(SerializedNode serializedNode, Func<string, T?> instantiator)
    {
        var id = serializedNode.Id;

        CompressedId compressedId;

        T? node;
        bool duplicateFound = false;

        do
        {
            compressedId = Compress(id);
            if (IsInDependentNodes(compressedId) && Handler.SkipDeserializingDependentNode(compressedId))
                return null;

            node = instantiator(id);
            if (node == null)
                return null;

            duplicateFound = false;

            if (_deserializedNodesById.TryGetValue(compressedId, out var existingNode))
            {
                id = Handler.DuplicateNodeId(compressedId, existingNode, node);
                if (id == null)
                    return null;
                duplicateFound = true;
            }
        } while (duplicateFound);

        _deserializedNodesById[compressedId] = node;
        return compressedId;
    }

    #region Containment

    /// Takes care of <see cref="IDeserializerHandler.CircularContainment"/>
    /// and <see cref="IDeserializerHandler.DuplicateContainment"/>.
    protected IWritableNode? PreventCircularContainment(T node, IWritableNode? result)
    {
        if (result == null)
            return null;

        while (result != null && ContainsAncestor(result, node))
            result = Handler.CircularContainment(result, node);

        if (result == null)
            return null;

        var existingParent = result.GetParent();
        if (existingParent != null && existingParent != node &&
            !Handler.DuplicateContainment(result, node, existingParent))
            return null;

        return result;
    }

    private bool ContainsAncestor(IWritableNode node, IReadableNode? parent) =>
        ReferenceEquals(node, parent) || parent != null && ContainsAncestor(node, parent.GetParent());

    #endregion

    #region Reference

    /// Tries to find a target node in this order:
    /// 1. <see cref="_deserializedNodesById"/>
    /// 2. <see cref="_dependentNodesById"/>
    /// 3. By resolveInfo in <see cref="ILionCoreLanguage"/>
    /// 4. By resolveInfo in <see cref="IBuiltInsLanguage"/>
    /// 5. By resolveInfo vs. name in <see cref="_deserializedNodesById"/> (depending on <see cref="ResolveInfoHandling"/>)
    protected IReadableNode? FindReferenceTarget(CompressedId? targetId, string? resolveInfo) =>
        (targetId, resolveInfo) switch
        {
            ({ } tid, _) =>
                _deserializedNodesById.TryGetValue(tid, out var ownNode)
                    ? ownNode
                    : _dependentNodesById.GetValueOrDefault(tid),
            (null, { } s) when s.StartsWith(ILionCoreLanguage.ResolveInfoPrefix) =>
                ResolveResolveInfo(s, ILionCoreLanguage.ResolveInfoPrefix, LionWebVersion.LionCore),
            (null, { } s) when s.StartsWith(IBuiltInsLanguage.ResolveInfoPrefix) =>
                ResolveResolveInfo(s, IBuiltInsLanguage.ResolveInfoPrefix, LionWebVersion.BuiltIns),
            (null, { } s) => ResolveByName(s),
            _ => null
        };

    private IKeyed? ResolveResolveInfo(string resolveInfo, string prefix, Language language)
    {
        var remainingResolveInfo = resolveInfo[prefix.Length..];
        var parts = remainingResolveInfo.Split(".");
        var entityName = parts[0];
        if (entityName == language.Name)
            return language;
        var entity = language.Entities.FirstOrDefault(e => e.Name == entityName);
        return entity == null || parts.Length <= 1
            ? entity
            : M1Extensions.Children(entity).FirstOrDefault(c => c.Name == parts[1]);
    }

    private IReadableNode? ResolveByName(string s)
    {
        var namedNodes = _dependentNodesById.Values
            .Concat(_deserializedNodesById.Values)
            .OfType<INamed>()
            .Where(n => n.CollectAllSetFeatures().Contains(_builtIns.INamed_name));
            
        return ResolveInfoHandling switch
        {
            ReferenceResolveInfoHandling.None => null,
            ReferenceResolveInfoHandling.Name => namedNodes.FirstOrDefault(n => n.Name == s),
            ReferenceResolveInfoHandling.NameIfUnique => GetSingle(namedNodes, n => n.Name == s)
        };
    }
    
    private static TSource? GetSingle<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        using IEnumerator<TSource> e = source.GetEnumerator();
        while (e.MoveNext())
        {
            TSource result = e.Current;
            if (predicate(result))
            {
                while (e.MoveNext())
                {
                    if (predicate(e.Current))
                    {
                        return default;
                    }
                }
                return result;
            }
        }

        return default;
    }


    /// Installs all of <paramref name="references"/> into <paramref name="nodeId"/>, if the target can be found.
    /// Takes care of <see cref="IDeserializerHandler.UnresolvableReferenceTarget"/>.
    /// and <see cref="IDeserializerHandler.InvalidReference"/>.
    protected void InstallReferences(CompressedId nodeId, IEnumerable<CompressedReference> references)
    {
        T node = _deserializedNodesById[nodeId];

        if (node is not IWritableNode writable)
        {
            Handler.InvalidReference(node);
            return;
        }

        foreach (var (compressedMetaPointer, targetEntries) in references)
        {
            var reference = _deserializerMetaInfo.FindFeature<Reference>(node, compressedMetaPointer);
            if (reference == null)
                continue;

            List<IReadableNode> targets = targetEntries
                .Select(target => FindReferenceTarget(node, reference, target.Item1, target.Item2))
                .Where(c => c != null)
                .ToList()!;

            SetLink(targets, writable, reference);
        }
    }

    private IReadableNode? FindReferenceTarget(IReadableNode node, Feature reference, CompressedId? targetId,
        string? resolveInfo) =>
        FindReferenceTarget(targetId, resolveInfo) ??
        Handler.UnresolvableReferenceTarget(targetId, resolveInfo, reference, node);

    /// Compresses <paramref name="r"/>.
    protected CompressedReference Compress(SerializedReference r) =>
    (
        Compress(r.Reference),
        r
            .Targets
            .Select(t => (CompressOpt(t.Reference), t.ResolveInfo))
            .ToList()
    );

    #endregion

    /// Sets <paramref name="link"/> inside <paramref name="node"/> to <paramref name="children"/>, if possible.
    /// Uses only the first entry of <paramref name="children"/> if <paramref name="link"/> is single.
    ///
    /// <para>
    /// Takes care of <see cref="IDeserializerHandler.InvalidLinkValue{T}"/>.
    /// </para>
    protected void SetLink<TChild>(List<TChild> children, IWritableNode node, Feature link)
        where TChild : class, IReadableNode
    {
        if (children.Count == 0)
            return;

        var single = link is Link { Multiple: false };
        try
        {
            node.Set(link, single && children.Count == 1 ? children[0] : children);
        } catch (InvalidValueException)
        {
            List<TChild>? replacement = Handler.InvalidLinkValue(children, link, node);
            if (replacement != null)
                node.Set(link, single ? replacement.FirstOrDefault() : replacement);
        }
    }

    /// Compresses <paramref name="id"/>.
    protected internal CompressedId Compress(string id) =>
        CompressedId.Create(id, StoreUncompressedIds);

    /// Compresses <paramref name="id"/> if not <c>null</c>.
    protected CompressedId? CompressOpt(string? id) =>
        id != null ? CompressedId.Create(id, StoreUncompressedIds) : null;

    /// Compresses <paramref name="metaPointer"/>.
    protected CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, StoreUncompressedIds);

    /// Checks whether <paramref name="compressedId"/> is contained in <see cref="_dependentNodesById"/>.
    protected bool IsInDependentNodes(CompressedId compressedId) =>
        _dependentNodesById.ContainsKey(compressedId);
}