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

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Partition;
using Core.M3;
using Core.Notification;
using Core.Notification.Partition;
using Core.Serialization;
using Message;
using Message.Command;

public abstract class DeltaCommandToEventMapperBase
{
    private SharedNodeMap _sharedNodeMap;
    private SharedKeyedMap _sharedKeyedMap;
    private DeserializerBuilder _deserializerBuilder;

    protected DeltaCommandToEventMapperBase(
        SharedNodeMap sharedNodeMap,
        SharedKeyedMap sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _sharedNodeMap = sharedNodeMap;
        _sharedKeyedMap = sharedKeyedMap;
        _deserializerBuilder = deserializerBuilder;
    }

    protected Property ToProperty(MetaPointer deltaProperty, IReadableNode node) =>
        ToFeature<Property>(deltaProperty, node);

    protected SemanticPropertyValue ToPropertyValue(IReadableNode node, Property property, PropertyValue value) =>
        _deserializerBuilder.Build().VersionSpecifics.ConvertDatatype(node, property, property.Type, value) ??
        throw new InvalidValueException(property, value);

    protected Containment ToContainment(MetaPointer deltaContainment, IReadableNode node) =>
        ToFeature<Containment>(deltaContainment, node);

    protected Reference ToReference(MetaPointer deltaReference, IReadableNode node) =>
        ToFeature<Reference>(deltaReference, node);

    protected IReferenceTarget ToTarget(TargetNode? targetNode, ResolveInfo? resolveInfo)
    {
        IReadableNode? target = null;
        if (targetNode != null &&
            _sharedNodeMap.TryGetValue(targetNode, out var node))
            target = node;

        return new ReferenceTarget(resolveInfo, target);
    }

    protected static INotificationId ToEventId(IDeltaCommand command) =>
        new ParticipationNotificationId(command.InternalParticipationId, command.CommandId);

    protected IWritableNode ToNode(TargetNode nodeId)
    {
        if (_sharedNodeMap.TryGetValue(nodeId, out var node))
        {
            if (node is IWritableNode w) return w;
        throw new NotImplementedException($"node: {node}");
        }

        // TODO change to correct exception 
        throw new NotImplementedException($"nodeId: {nodeId}");
    }

    private T ToFeature<T>(MetaPointer deltaReference, IReadableNode node) where T : Feature
    {
        if (_sharedKeyedMap.TryGetValue(Compress(deltaReference), out var e) && e is T c)
            return c;

        throw new UnknownFeatureException(node.GetClassifier(), deltaReference);
    }

    private CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, true);

    protected IWritableNode Deserialize(DeltaSerializationChunk deltaChunk)
    {
        var nodes = _deserializerBuilder.Build().Deserialize(deltaChunk.Nodes, _sharedNodeMap.Values);
        if (nodes is [IWritableNode w])
            return w;

        // TODO change to correct exception 
        throw new NotImplementedException();
    }
}