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

namespace LionWeb.Core.Test.Serialization.Protobuf;

using Core.Serialization;
using Io.Lionweb.Protobuf;
using M1;
using M2;
using M3;
using System.Collections;
using VersionSpecific.V2023_1;
using VersionSpecific.V2024_1_Compatible;

public class ProtobufSerializer
{
    private readonly IEnumerable<IReadableNode> _nodes;

    private readonly ISerializerVersionSpecifics _serializerSpecifics;

    private readonly HashSet<Language> _usedLanguages = new();
    private readonly Dictionary<string, int> _stringValues = new();
    private readonly Dictionary<MetaPointer, int> _metaPointers = new();
    private readonly Dictionary<IReadableNode, int> _nodeIndices = new();
    private readonly List<PBNode> _pbNodes = new();

    public ProtobufSerializer(IEnumerable<IReadableNode> nodes)
    {
        _nodes = nodes;

        _serializerSpecifics = new SerializerVersionSpecifics_2024_1_Compatible();
    }

    public PBChunk Serialize()
    {
        foreach (var node in _nodes)
        {
            CreateNode(node);
        }

        foreach (var pair in _nodeIndices)
        {
            var pbNode = _pbNodes[pair.Value];
            foreach (var reference in pair.Key.CollectAllSetFeatures().OfType<Reference>())
            {
                var pbRef = new PBReference { MetaPointerIndex = AsMetaPointer(reference.Type) };
                if (reference.Multiple)
                {
                    var targets = (IEnumerable)pair.Key.Get(reference);
                    foreach (var target in targets.Cast<IReadableNode>())
                    {
                        CreateTarget(target, pbRef);
                    }
                } else
                {
                    var target = (IReadableNode)pair.Key.Get(reference);
                    CreateTarget(target, pbRef);
                }

                pbNode.References.Add(pbRef);
            }
        }

        var result = new PBChunk { SerializationFormatVersion = _serializerSpecifics.Version.VersionString };
        result.Languages.AddRange(_usedLanguages.Select(l => new PBLanguage
        {
            Key = AsString(l.Key), Version = AsString(l.Version)
        }));
        result.MetaPointers.AddRange(_metaPointers.Keys.Select(metaPointer => new PBMetaPointer
        {
            Language = AsString(metaPointer.Language),
            Version = AsString(metaPointer.Version),
            Key = AsString(metaPointer.Key)
        }));
        result.StringValues.AddRange(_stringValues.Keys);
        result.Nodes.AddRange(_pbNodes);

        return result;
    }

    private void CreateTarget(IReadableNode target, PBReference pbRef)
    {
        var pbTarget = new PBReferenceValue();
        if (target != null)
        {
            if (_nodeIndices.TryGetValue(target, out var targetIndex))
                pbTarget.Referred = targetIndex;
            if (target is INamed n && n.TryGetName(out var name))
                pbTarget.ResolveInfo = AsString(name);
        }

        pbRef.Values.Add(pbTarget);
    }

    private PBNode CreateNode(IReadableNode node)
    {
        var result = new PBNode { Id = AsString(node.GetId()), Classifier = AsMetaPointer(node.GetClassifier()), };
        var ownIndex = _nodeIndices.Count;
        _nodeIndices[node] = ownIndex;
        _pbNodes.Add(result);

        foreach (var feature in node.CollectAllSetFeatures())
        {
            switch (feature)
            {
                case Property p:
                    result.Properties.Add(CreateProperty(p, node));
                    break;

                case Containment c:
                    var containmentIndex = AsMetaPointer(c.ToMetaPointer());
                    var pbContainment = new PBContainment { MetaPointerIndex = containmentIndex };
                    if (c.Multiple)
                    {
                        var children = (IEnumerable)node.Get(c);
                        foreach (var ch in children.Cast<IReadableNode>())
                        {
                            var pbCh = CreateNode(ch);
                            pbCh.Parent = ownIndex;
                            pbContainment.Children.Add(_nodeIndices[ch]);
                        }
                    } else
                    {
                        var child = (IReadableNode)node.Get(c);
                        var pbChild = CreateNode(child);
                        pbChild.Parent = ownIndex;
                        pbContainment.Children.Add(_nodeIndices[child]);
                    }

                    result.Containments.Add(pbContainment);
                    break;
            }
        }

        foreach (var ann in node.GetAnnotations())
        {
            var pbAnn = CreateNode(ann);
            pbAnn.Parent = ownIndex;
            result.Annotations.Add(_nodeIndices[ann]);
        }

        return result;
    }

    private PBProperty CreateProperty(Property prop, IReadableNode node) =>
        new()
        {
            MetaPointerIndex = AsMetaPointer(prop.ToMetaPointer()),
            Value = AsString(_serializerSpecifics.ConvertDatatype(node, prop, node.Get(prop)))
        };

    private int AsString(string? str)
    {
        if (str == null)
            return -1;

        if (_stringValues.TryGetValue(str, out var result))
            return result;

        result = _stringValues.Count;
        _stringValues[str] = result;

        return result;
    }

    private int AsMetaPointer(LanguageEntity entity)
    {
        RegisterLanguage(entity.GetLanguage());
        return AsMetaPointer(entity.ToMetaPointer());
    }

    private int AsMetaPointer(MetaPointer metaPointer)
    {
        if (_metaPointers.TryGetValue(metaPointer, out var result))
            return result;

        result = _metaPointers.Count;
        _metaPointers[metaPointer] = result;

        return result;
    }

    private void RegisterLanguage(Language language) =>
        _usedLanguages.Add(language);
}