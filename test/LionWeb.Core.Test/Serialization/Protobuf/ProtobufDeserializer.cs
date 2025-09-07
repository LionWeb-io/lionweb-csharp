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

using Core.Utilities;
using Io.Lionweb.Protobuf;
using M1;
using M2;
using M3;

public class ProtobufDeserializer
{
    private readonly PBChunk _chunk;

    private LionWebVersions _lionWebVersion = LionWebVersions.v2024_1_Compatible;

    private readonly Dictionary<int, Language> _languages = new();
    private readonly Dictionary<int, IKeyed> _entities = new();
    private readonly Dictionary<int, INode> _nodes = new();

    public ProtobufDeserializer(PBChunk chunk)
    {
        _chunk = chunk;
    }

    public IEnumerable<INode> Deserialize(IEnumerable<Language> languages)
    {
        var enumerable = languages.Append(_lionWebVersion.LionCore).Append(_lionWebVersion.BuiltIns).ToList();

        for (var i = 0; i < _chunk.Languages.Count; i++)
        {
            PBLanguage? pbLanguage = _chunk.Languages[i];
            var key = AsString(pbLanguage.Key);
            var version = AsString(pbLanguage.Version);
            _languages[i] = enumerable.FirstOrDefault(l => l.Key == key);
        }

        for (var i = 0; i < _chunk.MetaPointers.Count; i++)
        {
            var pbMetaPointer = _chunk.MetaPointers[i];
            var language = AsString(pbMetaPointer.Language);
            var version = AsString(pbMetaPointer.Version);
            var key = AsString(pbMetaPointer.Key);
            _entities[i] = _languages
                .Values
                .Where(l => l.Key == language && l.Version == version)
                .SelectMany(l => M1Extensions.Descendants((IReadableNode)l, true, false))
                .OfType<IKeyed>()
                .FirstOrDefault(e => e.Key == key);
        }

        for (var i = 0; i < _chunk.Nodes.Count; i++)
        {
            PBNode? pbNode = _chunk.Nodes[i];
            var node = Convert(pbNode);
            _nodes[i] = node;
        }

        for (var i = 0; i < _chunk.Nodes.Count; i++)
        {
            PBNode? pbNode = _chunk.Nodes[i];
            var node = _nodes[i];

            foreach (var pbContainment in pbNode.Containments)
            {
                var cont = (Containment)_entities[pbContainment.MetaPointerIndex];
                var value = pbContainment.Children.Select(idx => _nodes[idx]).ToList();
                if (cont.Multiple)
                {
                    node.Set(cont, value);
                } else
                {
                    node.Set(cont, value.FirstOrDefault());
                }
            }

            foreach (var pbReference in pbNode.References)
            {
                var reference = (Reference)_entities[pbReference.MetaPointerIndex];
                var targets = pbReference.Values.Select(value => _nodes[value.Referred]).ToList();
                if (reference.Multiple)
                {
                    node.Set(reference, targets);
                } else
                {
                    node.Set(reference, targets.FirstOrDefault());
                }
            }

            node.AddAnnotations(pbNode.Annotations.Select(ann => _nodes[ann]));
        }

        return _nodes.Values.Where(n => n.GetParent() == null);
    }

    private INode Convert(PBNode pbNode)
    {
        var classifier = AsClassifier(pbNode.Classifier);
        var result = classifier.GetLanguage().GetFactory().CreateNode(AsString(pbNode.Id), classifier);

        foreach (var pbProperty in pbNode.Properties)
        {
            var prop = (Property)_entities[pbProperty.MetaPointerIndex];
            var rawValue = AsString(pbProperty.Value);
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

        return result;
    }

    private string? AsString(int index)
    {
        if (index == -1)
            return null;

        return _chunk.StringValues[index];
    }

    private Classifier AsClassifier(int index) =>
        (Classifier)_entities[index];
}