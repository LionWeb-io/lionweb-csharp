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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace LionWeb.Core.Utilities;

using M1;
using M2;
using M3;
using Serialization;
using System.Security.Cryptography;

/// <summary>
/// Hashes all input nodes, including all their descendants and annotations.
/// Does NOT consider the node's <see cref="IReadableNode.GetId">nodeId</see>
/// (except for <i>external reference targets</i>, see below).
/// 
/// <para>
/// We consider a reference target _internal_ if the target is part of the input nodes.
/// Otherwise, we consider the target _external_.
/// </para>
/// 
/// <list type="bullet">
///   <listheader>Detected changes:</listheader>
///   <item>Classifier: meta-pointer</item>
///   <item>Node: classifier, features, annotations (neither node id nor parent)</item>
///   <item>list of nodes: order, each node</item> 
///   <item>annotations: same as list of nodes</item>
///   <item>feature: meta-pointer, value</item>
///   <item>property: serialized value</item>
///   <item>single containment: same node</item>
///   <item>multiple containment: same list of nodes</item>
///   <item>single reference with internal target: internal index</item> 
///   <item>single reference with external target: node id</item> 
///   <item>multiple reference: same reference at each position</item>
/// </list>
/// </summary>
public class Hasher
{
    private readonly IList<IReadableNode> _nodes;
    private readonly Func<IReadableNode, Feature, object?, string?> _datatypeConverter;

    /// We need to treat internal and external references differently.
    /// However, we might encounter a node first as reference target, and only later as actual node
    /// (turning it into an internal reference).
    /// Thus, we hash the index in this lookup table for each reference (<see cref="LookupReferenceIndex"/>).
    /// At the end, we attach all external node ids in order of their index.
    private readonly Dictionary<string, ReferenceIndex> _referenceIndices = [];

    private int _nextReferenceIndex = 0;

    private readonly SHA256 _hash = SHA256.Create();

    /// <param name="nodes">List of nodes to hash. Order matters.</param>
    /// <param name="lionWebVersion">Version of LionWeb standard to use. Required to compare property values.</param>
    /// <inheritdoc cref="Hasher"/>
    public Hasher(IList<IReadableNode> nodes, LionWebVersions? lionWebVersion = null)
    {
        _nodes = nodes;
        var version = lionWebVersion ?? LionWebVersions.Current;
        _datatypeConverter = ISerializerVersionSpecifics.Create(version).ConvertDatatype;
    }

    /// Calculates the hash of all input nodes.
    public IHash Hash()
    {
        try
        {
            Nodes(_nodes);

            AppendExternalReferenceTargets();

            _hash.TransformFinalBlock([], 0, 0);
            return new ByteArrayHash(nameof(SHA256), _hash.Hash!);
        } finally
        {
            _hash.Dispose();
        }
    }

    protected virtual void Nodes(IEnumerable<IReadableNode> nodes)
    {
        foreach (var node in nodes)
        {
            Node(node);
        }
    }

    protected virtual void Node(IReadableNode node)
    {
        RegisterNode(node);
        Classifier(node.GetClassifier());

        foreach (var feature in node
                     .CollectAllSetFeatures()
                     .OrderBy(f => f.Key)
                     .ThenBy(f => f.GetLanguage().Key)
                     .ThenBy(f => f.GetLanguage().Version)
                )
        {
            Feature(node, feature);
        }

        Annotations(node);
    }

    protected virtual void Classifier(Classifier classifier) =>
        MetaPointer(classifier.ToMetaPointer());

    protected virtual void MetaPointer(MetaPointer metaPointer)
    {
        Hash(metaPointer.Key);
        Hash(metaPointer.Language);
        Hash(metaPointer.Version);
    }

    protected virtual void Annotations(IReadableNode node)
    {
        Hash("annotations");
        Nodes(node.GetAnnotations());
    }

    protected virtual void Feature(IReadableNode node, Feature feature)
    {
        MetaPointer(feature.ToMetaPointer());

        switch (feature)
        {
            case Property p: Property(node, p); break;
            case Containment c: Containment(node, c); break;
            case Reference r: References(node, r); break;
        }
    }

    protected virtual void Property(IReadableNode node, Property property)
    {
        var rawValue = _datatypeConverter.Invoke(node, property, node.Get(property));
        if (rawValue != null)
            Hash(rawValue);
    }

    protected virtual void Containment(IReadableNode node, Containment containment) =>
        Nodes(containment.AsNodes<IReadableNode>(node.Get(containment)));

    protected virtual void References(IReadableNode node, Reference reference)
    {
        foreach (var target in reference.AsNodes<IReadableNode>(node.Get(reference)))
        {
            Reference(node, reference, target);
        }
    }

    protected virtual void Reference(IReadableNode node, Reference reference, IReadableNode target) =>
        Hash(LookupReferenceIndex(target).ToString());

    protected void Hash(string str)
    {
        var bytes = ByteExtensions.AsUtf8Bytes(str);
        _hash.TransformBlock(bytes, 0, bytes.Length, null, 0);
    }

    #region reference handling

    private void HashSeparator() =>
        _hash.TransformBlock([0], 0, 1, null, 0);

    private const int _indexUnset = -1;

    private void RegisterNode(IReadableNode node)
    {
        if (_referenceIndices.TryGetValue(node.GetId(), out var index))
        {
            index.Internal = true;
            return;
        }

        _referenceIndices[node.GetId()] = new ReferenceIndex(_indexUnset, true);
    }

    private int LookupReferenceIndex(IReadableNode node)
    {
        if (_referenceIndices.TryGetValue(node.GetId(), out var index))
        {
            if (index.Index == _indexUnset)
            {
                index.Index = ++_nextReferenceIndex;
            }

            return index.Index;
        }

        _referenceIndices[node.GetId()] = new ReferenceIndex(++_nextReferenceIndex, false);

        return _nextReferenceIndex;
    }

    private void AppendExternalReferenceTargets()
    {
        Hash("referenceIndices");
        foreach (var pair in _referenceIndices
                     .Where(p => !p.Value.Internal)
                     .OrderBy(p => p.Value.Index)
                )
        {
            Hash(pair.Key);
            // if we don't put a separator between entries, external references to node ids {TT, T} and {T, TT} lead to identical hashes.
            HashSeparator();
        }
    }

    private record struct ReferenceIndex(int Index, bool Internal);

    #endregion
}

/// A small piece of data representing the state of an ordered set of nodes.
/// Any change to the underlying nodes results in a different hash.
/// <seealso cref="Hasher"/>.
public interface IHash : IEquatable<IHash>;

/// <inheritdoc cref="IHash" />
public readonly record struct ByteArrayHash : IHash
{
    /// <inheritdoc cref="IHash" />
    /// <param name="Algorithm">Algorithm used to create the hash. Used as prefix in <see cref="ToString"/>.</param>
    /// <param name="Hash">Hash code.</param>
    public ByteArrayHash(string Algorithm, byte[] Hash)
    {
        if(Hash.Length < 4)
            throw new ArgumentException("Hash must contain at least 4 bytes.");
        
        this.Algorithm = Algorithm;
        this.Hash = Hash;
    }

    /// <inheritdoc />
    public override string ToString() =>
        Algorithm + "_" + BitConverter.ToString(Hash);

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(Algorithm, BitConverter.ToInt32(Hash, 0));

    /// <inheritdoc />
    public bool Equals(IHash? other) =>
        other is ByteArrayHash b && Equals(b);

    /// <inheritdoc />
    public bool Equals(ByteArrayHash other) =>
        Algorithm.Equals(other.Algorithm) && ByteExtensions.Equals(Hash, other.Hash);

    /// Algorithm used to create the hash. Used as prefix in <see cref="ToString"/>.
    public string Algorithm { get; init; }

    /// Hash code.
    public byte[] Hash { get; init; }
}