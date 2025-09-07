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

namespace LionWeb.Core.Test.Serialization.Protobuf.Streaming;

using Core.Serialization;
using Core.Utilities;
using Io.Lionweb.Protobuf;
using Io.Lionweb.Protobuf.Streaming;
using M1;
using M2;
using M3;
using System.Diagnostics;
using StringIndex = ulong;
using LanguageIndex = ulong;
using MetaPointerIndex = ulong;
using NodeIndex = ulong;
using ResolveInfo = string;
using CompressedContainment = (ulong, List<ulong>);
using CompressedReference = (ulong, List<(ulong?, ulong?)>);

public abstract class ProtobufDeserializerBase
{
    private readonly PBChunk _chunk;

    private LionWebVersions _lionWebVersion = LionWebVersions.v2024_1_Compatible;

    private readonly List<Language> _registeredLanguages;
    private readonly LanguageLookup _languages;
    private readonly StringLookup _strings;
    private readonly MetaPointerLookup _entities;
    private readonly Dictionary<NodeIndex, INode> _nodes = new();
    private readonly Dictionary<NodeIndex, NodeId> _unboundNodeIds = new();

    private readonly Dictionary<NodeIndex, List<CompressedContainment>> _containmentsByOwnerId = new();

    private readonly Dictionary<NodeIndex, List<CompressedReference>> _referencesByOwnerId = new();
    private readonly Dictionary<NodeIndex, List<NodeIndex>> _annotationsByOwnerId = new();

    public ProtobufDeserializerBase(IEnumerable<Language> languages)
    {
        _registeredLanguages = languages.ToList();

        _languages = new(_lionWebVersion);
        _entities = new(_lionWebVersion);
        _strings = new();
    }

    public IEnumerable<INode> Finish()
    {
        foreach (var (ownerIdx, psChildren) in _containmentsByOwnerId)
        {
            if (TryGetNode(ownerIdx, out var n) && n is INode node)
            {
                foreach (var psChild in psChildren)
                {
                    var containment = (Containment) _entities.Get(psChild.Item1);
                    var children = psChild.Item2
                        .Select(idx => TryGetNode(idx, out var r) ? r : null)
                        .Where(n => n is not null)
                        .ToList();
                    
                    if (containment.Multiple)
                        node.Set(containment, children);
                    else
                        node.Set(containment, children.FirstOrDefault());
                }
            }
        }
        
        foreach (var (ownerIdx, psTargets) in _referencesByOwnerId)
        {
            if (TryGetNode(ownerIdx, out var n) && n is INode node)
            {
                foreach (var psReference in psTargets)
                {
                    var reference = (Reference) _entities.Get(psReference.Item1);
                    var targets = psReference.Item2
                        .Select(pair => TryGetNode((NodeIndex)pair.Item1, out var r) ? r : null)
                        .Where(n => n is not null)
                        .ToList();
                    
                    if (reference.Multiple)
                        node.Set(reference, targets);
                    else
                        node.Set(reference, targets.FirstOrDefault());
                }
            }
        }
        
        foreach (var (ownerIdx, psAnns) in _annotationsByOwnerId)
        {
            if (TryGetNode(ownerIdx, out var n) && n is INode node)
            {
                    var anns = psAnns
                        .Select(idx => TryGetNode(idx, out var r) ? r : null)
                        .Where(n => n is not null)
                        .ToList();
                
                    node.AddAnnotations(anns.Cast<INode>());
            }
        }

        return _nodes.Values.Where(n => n.GetParent() == null);
    }

    protected void Process(PsMessage message)
    {
        switch (message)
        {
            case { SerializationFormatVersion: { } v }:
                Process(v);
                break;

            case { Language: { } l }:
                Process(l);
                break;

            case { MetaPointer: { } m }:
                Process(m);
                break;

            case { StringValue: { } s }:
                Process(s);
                break;

            case { UnboundId: { } u }:
                Process(u);
                break;

            case { Node: { } n }:
                Process(n);
                break;
        }
    }

    protected void Process(PsSerializationFormatVersion v) => Debug.Assert(v.Version == _lionWebVersion.VersionString);

    protected void Process(PsLanguage l)
    {
        var version = AsString(l.Version);
        var key = AsString(l.Key);
        _languages.Register(
            _registeredLanguages.FirstOrDefault(lang => key == lang.Key && version == lang.Version), l.Idx);
    }

    protected void Process(PsMetaPointer m)
    {
        var language = AsString(m.Language);
        var version = AsString(m.Version);
        var key = AsString(m.Key);
        _entities.Register(_registeredLanguages
            .Where(l => l.Key == language && l.Version == version)
            .SelectMany(l => M1Extensions.Descendants((IReadableNode)l, true, false))
            .OfType<IKeyed>()
            .FirstOrDefault(e => e.Key == key), m.Idx);
    }

    protected void Process(PsStringValue s) => _strings.Register(s.Value, s.Idx);

    protected void Process(PsUnboundNodeId u) => _unboundNodeIds[u.Idx] = AsString(u.Id);

    protected void Process(PsNode n) => Convert(n);

    private INode Convert(PsNode psNode)
    {
        var classifier = AsClassifier(psNode.Classifier);
        var result = classifier.GetLanguage().GetFactory().CreateNode(AsString(psNode.Id), classifier);
        _nodes[psNode.Idx] = result;

        foreach (var psProperty in psNode.Properties)
        {
            var prop = (Property)_entities.Get(psProperty.MetaPointerIndex);
            var rawValue = AsString(psProperty.Value);
            object value = rawValue;
            if (prop.Type.EqualsIdentity(_lionWebVersion.BuiltIns.Boolean))
            {
                value = bool.Parse(rawValue);
            } else if (prop.Type.EqualsIdentity(_lionWebVersion.BuiltIns.Integer))

            {
                value = int.Parse(rawValue);
            }

            result.Set(prop, value);
        }

        List<CompressedContainment> deferredContainments = [];

        foreach (var psContainment in psNode.Containments)
        {
            var cont = (Containment)_entities.Get(psContainment.MetaPointerIndex);

            List<IReadableNode> children = [];
            bool allResolved = true;
            foreach (var psChildIdx in psContainment.Children)
            {
                if (!TryGetNode(psChildIdx, out var child))
                {
                    allResolved = false;
                    break;
                }

                children.Add(child);
            }

            if (allResolved)
            {
                if (cont.Multiple)
                {
                    result.Set(cont, children);
                } else
                {
                    result.Set(cont, children.FirstOrDefault());
                }
            } else
            {
                deferredContainments.Add(Compress(psContainment));
            }
        }

        if (deferredContainments.Count != 0)
        {
            _containmentsByOwnerId[psNode.Idx] = deferredContainments;
        }

        List<CompressedReference> deferredReferences = [];
        
        foreach (var psReference in psNode.References)
        {
            var reference = (Reference)_entities.Get(psReference.MetaPointerIndex);

            List<IReadableNode> targets = [];
            bool allResolved = true;
            foreach (var psReferenceValue in psReference.Values)
            {
                if (!TryGetNode(psReferenceValue.Referred, out var target))
                {
                    allResolved = false;
                    break;
                }

                targets.Add(target);
            }

            if (allResolved)
            {
                if (reference.Multiple)
                {
                    result.Set(reference, targets);
                } else
                {
                    result.Set(reference, targets.FirstOrDefault());
                }
            } else
            {
                deferredReferences.Add(Compress(psReference));
            }
        }

        if (deferredReferences.Count != 0)
        {
            _referencesByOwnerId[psNode.Idx] = deferredReferences;
        }

        {
            List<IReadableNode> annotations = [];
            bool allResolved = true;
            foreach (var psAnnIdx in psNode.Annotations)
            {
                if (!TryGetNode(psAnnIdx, out var ann))
                {
                    allResolved = false;
                    break;
                }

                annotations.Add(ann);
            }

            if (allResolved)
            {
                result.AddAnnotations(annotations.Cast<INode>());
            } else
            {
                _annotationsByOwnerId[psNode.Idx] = psNode.Annotations.ToList();
            }
        }

        return result;
    }

    private string? AsString(StringIndex idx) =>
        _strings.Get(idx);

    private Classifier AsClassifier(MetaPointerIndex idx) =>
        (Classifier)_entities.Get(idx);

    private bool TryGetNode(NodeIndex idx, out IReadableNode? result)
    {
        if (_nodes.TryGetValue(idx, out var r))
        {
            result = r;
            return true;
        }


        if (_unboundNodeIds.ContainsKey(idx))
        {
            result = null;
            return true;
        }

        result = null;
        return false;
    }

    /// Compresses <paramref name="id"/>.
    protected internal ICompressedId Compress(NodeId id) =>
        ICompressedId.Create(id, CompressedIdConfig);

    /// Compresses <paramref name="id"/> if not <c>null</c>.
    protected internal ICompressedId? CompressOpt(NodeId? id) =>
        id != null ? ICompressedId.Create(id, CompressedIdConfig) : null;

    /// Compresses <paramref name="metaPointer"/>.
    protected CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, CompressedIdConfig);

    private CompressedContainment Compress(PsContainment c) =>
    (
        c.MetaPointerIndex,
        c
            .Children
            .ToList()
    );

    private CompressedReference Compress(PsReference r) =>
    (
        r.MetaPointerIndex,
        r
            .Values
            .Select(t => ((NodeIndex?)t.Referred, (StringIndex?) t.ResolveInfo))
            .ToList()
    );
    private CompressedIdConfig CompressedIdConfig
    {
        get => new();
    }
}