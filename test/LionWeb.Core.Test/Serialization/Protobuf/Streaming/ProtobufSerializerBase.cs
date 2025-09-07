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
using Io.Lionweb.Protobuf.Streaming;
using M1;
using M2;
using M3;
using System.Collections;
using StringIndex = ulong;
using LanguageIndex = ulong;
using MetaPointerIndex = ulong;
using NodeIndex = ulong;

public abstract class ProtobufSerializerBase
{
    private readonly ISerializerVersionSpecifics _serializerSpecifics;
    protected readonly LionWebVersions _lionWebVersion;

    private readonly LanguageIndexer _languages;
    private readonly MetaPointerIndexer _metaPointers;
    private readonly StringIndexer _strings;
    private readonly Dictionary<NodeId, NodeIndex> _nodes;
    private NodeIndex _nextNodeIndex = 0;

    public ProtobufSerializerBase()
    {
        _lionWebVersion = LionWebVersions.v2024_1_Compatible;
        _serializerSpecifics = ISerializerVersionSpecifics.Create(_lionWebVersion);

        _languages = new(_lionWebVersion,
            (language, idx) => AddUsedLanguage(new PsLanguage
            {
                Idx = idx, Key = AsString(language.Key), Version = AsString(language.Version)
            }));
        _metaPointers = new(_lionWebVersion,
            (metaPointer, idx) => AddMetaPointer(new PsMetaPointer
            {
                Idx = idx,
                Language = AsString(metaPointer.Language),
                Version = AsString(metaPointer.Version),
                Key = AsString(metaPointer.Key)
            }));
        _strings = new((str, idx) => AddString(new PsStringValue { Idx = idx, Value = str }));
        _nodes = new();
    }

    public abstract PsNode Process(IReadableNode node);

    // public IEnumerable<PsMessage> Finish()
    // {
    //     foreach (var node in _nodes)
    //     {
    //         CreateNode(node);
    //     }
    //
    //     foreach (var pair in _nodeIndices)
    //     {
    //         var pbNode = _pbNodes[pair.Value];
    //         foreach (var reference in pair.Key.CollectAllSetFeatures().OfType<Reference>())
    //         {
    //             var pbRef = new PsReference { MetaPointerIndex = AsMetaPointer(reference.Type) };
    //             if (reference.Multiple)
    //             {
    //                 var targets = (IEnumerable)pair.Key.Get(reference);
    //                 foreach (var target in targets.Cast<IReadableNode>())
    //                 {
    //                     CreateTarget(target, pbRef);
    //                 }
    //             } else
    //             {
    //                 var target = (IReadableNode)pair.Key.Get(reference);
    //                 CreateTarget(target, pbRef);
    //             }
    //
    //             pbNode.References.Add(pbRef);
    //         }
    //     }
    //
    //     var result = new PsChunk { SerializationFormatVersion = _serializerSpecifics.Version.VersionString };
    //     result.Languages.AddRange(_usedLanguages.Select(l => new PsLanguage
    //     {
    //         Key = AsString(l.Key), Version = AsString(l.Version)
    //     }));
    //     result.MetaPointers.AddRange(_metaPointers.Keys.Select(metaPointer => new PsMetaPointer
    //     {
    //         Language = AsString(metaPointer.Language),
    //         Version = AsString(metaPointer.Version),
    //         Key = AsString(metaPointer.Key)
    //     }));
    //     result.StringValues.AddRange(_stringValues.Keys);
    //     result.Nodes.AddRange(_pbNodes);
    //
    //     return result;
    // }

    protected PsNode CreateNode(IReadableNode node)
    {
        var ownIndex = RegisterNode(node.GetId());
        var result = new PsNode
        {
            Idx = ownIndex, Id = AsString(node.GetId()), Classifier = AsMetaPointer(node.GetClassifier()),
        };

        if (node.GetParent() is { } parent)
        {
            result.Parent = RegisterNode(parent.GetId());
        }

        foreach (var feature in node.CollectAllSetFeatures())
        {
            switch (feature)
            {
                case Property p:
                    result.Properties.Add(CreateProperty(p, node));
                    break;

                case Containment c:
                    var containmentIndex = AsMetaPointer(c.ToMetaPointer());
                    var pbContainment = new PsContainment { MetaPointerIndex = containmentIndex };
                    if (c.Multiple)
                    {
                        var children = (IEnumerable)node.Get(c);
                        foreach (var ch in children.Cast<IReadableNode>())
                        {
                            var childIndex = RegisterNode(ch.GetId());
                            pbContainment.Children.Add(childIndex);
                        }
                    } else
                    {
                        var child = (IReadableNode)node.Get(c);
                        var childIndex = RegisterNode(child.GetId());
                        pbContainment.Children.Add(childIndex);
                    }

                    result.Containments.Add(pbContainment);
                    break;

                case Reference reference:
                    var pbRef = new PsReference { MetaPointerIndex = AsMetaPointer(reference.ToMetaPointer()) };
                    if (reference.Multiple)
                    {
                        var targets = (IEnumerable)node.Get(reference);
                        foreach (var target in targets.Cast<IReadableNode>())
                        {
                            CreateTarget(target, pbRef);
                        }
                    } else
                    {
                        var target = (IReadableNode)node.Get(reference);
                        CreateTarget(target, pbRef);
                    }

                    result.References.Add(pbRef);
                    break;
            }
        }

        result.Annotations.AddRange(
            node
                .GetAnnotations()
                .Select(ann => ann.GetId())
                .Select(RegisterNode)
        );

        return result;
    }

    private PsProperty CreateProperty(Property prop, IReadableNode node) =>
        new()
        {
            MetaPointerIndex = AsMetaPointer(prop.ToMetaPointer()),
            Value = AsString(_serializerSpecifics.ConvertDatatype(node, prop, node.Get(prop)))
        };

    private void CreateTarget(IReadableNode? target, PsReference pbRef)
    {
        var pbTarget = new PsReferenceValue();
        if (target != null)
        {
            pbTarget.Referred = RegisterNode(target.GetId());
            if (target is INamed n && n.TryGetName(out var name))
                pbTarget.ResolveInfo = AsString(name);
        }

        pbRef.Values.Add(pbTarget);
    }

    private NodeIndex RegisterNode(NodeId nodeId)
    {
        if (_nodes.TryGetValue(nodeId, out var result))
            return result;

        result = _nextNodeIndex++;
        _nodes[nodeId] = result;

        return result;
    }

    private StringIndex AsString(string? str) => _strings.GetOrCreate(str);
    protected abstract void AddString(PsStringValue stringValue);

    private MetaPointerIndex AsMetaPointer(LanguageEntity entity)
    {
        RegisterLanguage(entity.GetLanguage());
        return AsMetaPointer(entity.ToMetaPointer());
    }

    private MetaPointerIndex AsMetaPointer(MetaPointer metaPointer) => _metaPointers.GetOrCreate(metaPointer);
    protected abstract void AddMetaPointer(PsMetaPointer metaPointer);


    private LanguageIndex RegisterLanguage(Language language) => _languages.GetOrCreate(language);
    protected abstract void AddUsedLanguage(PsLanguage language);
}