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
using CompressedReference = (CompressedMetaPointer, List<(ICompressedId? compressedId, ResolveInfo? resolveInfo)>);

/// <inheritdoc />
/// <typeparam name="T">Type of node to return</typeparam>
/// <typeparam name="H">Type of <see cref="IDeserializerHandler"/> to use.</typeparam>
public abstract class DeserializerBase<T, H> : IDeserializer<T>
    where T : class, IReadableNode where H : class, IDeserializerHandler
{
    /// Version of LionWeb standard to use.
    internal readonly IDeserializerVersionSpecifics _versionSpecifics;

    /// LionCore M3 according to <see cref="_versionSpecifics"/>.
    protected readonly ILionCoreLanguage _m3;

    /// LionCore builtins according to <see cref="_versionSpecifics"/>.
    protected readonly IBuiltInsLanguage _builtIns;

    ///Handler to customize this deserializer's behaviour in non-regular situations.
    protected readonly H _handler;

    /// <inheritdoc cref="DeserializerMetaInfo"/>
    protected readonly DeserializerMetaInfo _deserializerMetaInfo;

    /// <inheritdoc cref="IDeserializer.RegisterDependentNodes"/>
    protected readonly Dictionary<ICompressedId, IReadableNode> _dependentNodesById = new();

    /// Already deserialized nodes.
    protected readonly Dictionary<ICompressedId, T> _deserializedNodesById = new();

    /// <param name="lionWebVersion">Version of LionWeb standard to use.</param>
    /// <param name="handler">Optional handler to customize this deserializer's behaviour in non-regular situations.</param>
    /// <param name="compressedIdConfig">How to store compressed <see cref="IReadableNode.GetId()">node ids</see> and <see cref="MetaPointer">MetaPointers</see> during deserialization.</param>
    protected DeserializerBase(LionWebVersions lionWebVersion, H handler, CompressedIdConfig compressedIdConfig)
    {
        _m3 = lionWebVersion.LionCore;
        _builtIns = lionWebVersion.BuiltIns;
        _handler = handler;
        _deserializerMetaInfo = new DeserializerMetaInfo(_handler);
        _deserializerMetaInfo.CompressedIdConfig = compressedIdConfig;
        _versionSpecifics = IDeserializerVersionSpecifics.Create(lionWebVersion, this, _deserializerMetaInfo, _handler);
        _versionSpecifics.RegisterBuiltins();
    }

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get => _versionSpecifics.Version; }

    /// <inheritdoc />
    IDeserializerVersionSpecifics IDeserializer.VersionSpecifics => _versionSpecifics;

    /// How to store compressed <see cref="IReadableNode.GetId()">node ids</see> and <see cref="MetaPointer">MetaPointers</see> during deserialization.
    public CompressedIdConfig CompressedIdConfig
    {
        get => _deserializerMetaInfo.CompressedIdConfig;
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
    protected ICompressedId? ProcessInternal(SerializedNode serializedNode, Func<NodeId, T?> instantiator)
    {
        var id = serializedNode.Id;

        ICompressedId compressedId;

        T? node;
        bool duplicateFound = false;

        do
        {
            compressedId = Compress(id);
            if (IsInDependentNodes(compressedId) && _handler.SkipDeserializingDependentNode(compressedId))
                return null;

            node = instantiator(id);
            if (node == null)
                return null;

            duplicateFound = false;

            if (_deserializedNodesById.TryGetValue(compressedId, out var existingNode))
            {
                id = _handler.DuplicateNodeId(compressedId, existingNode, node);
                if (id == null)
                    return null;
                duplicateFound = true;
            }
        } while (duplicateFound);

        _deserializedNodesById[compressedId] = node;
        return compressedId;
    }

    #region Containment

    /// Sets <paramref name="containment"/> inside <paramref name="node"/> to <paramref name="compressedChildrenIds"/>, if possible.
    /// Uses only the first entry of <paramref name="compressedChildrenIds"/> if <paramref name="containment"/> is single.
    ///
    /// <para>
    /// Takes care of <see cref="IDeserializerHandler.InvalidLinkValue{T}"/>.
    /// </para>
    protected void InstallContainment(List<ICompressedId> compressedChildrenIds, IWritableNode node, Feature containment)
    {
        List<IWritableNode> children = compressedChildrenIds
            .Select<ICompressedId, IWritableNode?>(childId => FindChild(node, containment, childId))
            .Where(c => c != null)
            .ToList()!;

        SetContainment(children, node, containment);
    }

    private void SetContainment<TChild>(List<TChild> children, IWritableNode node, Feature containment)
        where TChild : class, IReadableNode
    {
        if (children.Count == 0)
            return;

        var single = containment is Containment { Multiple: false };
        try
        {
            node.Set(containment, single && children.Count == 1 ? children[0] : children);
        } catch (InvalidValueException)
        {
            List<TChild>? replacement = _handler.InvalidLinkValue(children, containment, node);
            if (replacement != null)
                node.Set(containment, single ? replacement.FirstOrDefault() : replacement);
        }
    }

    private IWritableNode? FindChild(IWritableNode node, Feature containment, ICompressedId childId)
    {
        IWritableNode? result = _deserializedNodesById.TryGetValue(childId, out var existingChild)
            ? existingChild as IWritableNode
            : _handler.UnresolvableChild(childId, containment, node);

        return PreventCircularContainment(node, result);
    }

    /// Takes care of <see cref="IDeserializerHandler.CircularContainment"/>
    /// and <see cref="IDeserializerHandler.DuplicateContainment"/>.
    protected IWritableNode? PreventCircularContainment(IWritableNode node, IWritableNode? result)
    {
        if (result == null)
            return null;

        while (result != null && ContainsAncestor(result, node))
            result = _handler.CircularContainment(result, node);

        if (result == null)
            return null;

        var existingParent = result.GetParent();
        if (existingParent != null && existingParent != node &&
            !_handler.DuplicateContainment(result, node, existingParent))
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
    protected IReadableNode? FindReferenceTarget(ICompressedId? targetId, ResolveInfo? resolveInfo) =>
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

    private IKeyed? ResolveResolveInfo(ResolveInfo resolveInfo, string prefix, Language language)
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

    /// <inheritdoc />
    void IDeserializer.InstallNodeReferences(ICompressedId nodeId, IEnumerable<CompressedReference> references) =>
        InstallReferences(nodeId, references);
    
    /// <inheritdoc cref="IDeserializer.InstallNodeReferences"/>
    protected void InstallReferences(ICompressedId nodeId, IEnumerable<CompressedReference> references)
    {
        T node = _deserializedNodesById[nodeId];

        if (node is not IWritableNode writable)
        {
            _handler.InvalidReference(node);
            return;
        }

        foreach (var (compressedMetaPointer, targetEntries) in references)
        {
            var feature = _deserializerMetaInfo.FindFeature<Reference>(node, compressedMetaPointer);
            switch (feature)
            {
                case null:
                    continue;
                case Containment c:
                    // required if FindFeature() "heals" the reference into a containment.
                    InstallContainment(
                        targetEntries
                            .Select(e => e.compressedId)
                            .Where(i => i is not null)
                            .ToList()!,
                        writable,
                        c
                    );
                    continue;
                default:
                    {
                        List<IReferenceTarget> targets = targetEntries
                            .Select(target =>
                                FindReferenceTarget(node, feature, target.compressedId, target.resolveInfo))
                            .Where(d => d is not null)
                            .ToList()!;

                        SetReference(targets, writable, feature);
                        break;
                    }
            }
        }
    }

    /// Sets <paramref name="reference"/> inside <paramref name="node"/> to <paramref name="targets"/>, if possible.
    /// Uses only the first entry of <paramref name="targets"/> if <paramref name="reference"/> is single.
    ///
    /// <para>
    /// Takes care of <see cref="IDeserializerHandler.InvalidLinkValue{T}"/>.
    /// </para>
    private void SetReference(List<IReferenceTarget> targets, IWritableNode node, Feature reference)
    {
        if (targets.Count == 0)
            return;

        var single = reference is Reference { Multiple: false };
        try
        {
            node.Set(reference, single && targets.Count == 1 ? targets[0] : targets);
        } catch (InvalidValueException)
        {
            List<T>? replacement = _handler.InvalidLinkValue(M2Extensions.AsNodes<T>(targets.Select(r => r.Target).Where(t => t is not null), reference).ToList(), reference, node);
            if (replacement != null)
                node.Set(reference, single ? replacement.FirstOrDefault() : replacement);
        }
    }

    private IReferenceTarget? FindReferenceTarget(IReadableNode node, Feature reference, ICompressedId? targetId,
        ResolveInfo? resolveInfo)
    {
        var target = FindReferenceTarget(targetId, resolveInfo);
        if (target is not null)
            return new ReferenceTarget(resolveInfo, target?.GetId() ?? targetId?.Original, target);

        return _handler.UnresolvableReferenceTarget(new ReferenceTarget(resolveInfo, targetId?.Original, null), reference, node);
    }

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

    /// Compresses <paramref name="id"/>.
    protected internal ICompressedId Compress(NodeId id) =>
        ICompressedId.Create(id, CompressedIdConfig);

    /// Compresses <paramref name="id"/> if not <c>null</c>.
    protected internal ICompressedId? CompressOpt(NodeId? id) =>
        id != null ? ICompressedId.Create(id, CompressedIdConfig) : null;

    /// Compresses <paramref name="metaPointer"/>.
    protected CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, CompressedIdConfig);

    /// Checks whether <paramref name="compressedId"/> is contained in <see cref="_dependentNodesById"/>.
    protected bool IsInDependentNodes(ICompressedId compressedId) =>
        _dependentNodesById.ContainsKey(compressedId);
}