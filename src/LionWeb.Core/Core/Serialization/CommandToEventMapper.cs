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

namespace LionWeb.Core.Serialization;

using M1;
using M2;
using M3;

public class CommandToEventMapper
{
    private readonly Dictionary<NodeId, IReadableNode> _sharedNodeMap;

    public CommandToEventMapper(Dictionary<NodeId, IReadableNode> sharedNodeMap)
    {
        _sharedNodeMap = sharedNodeMap;
    }

    public static Dictionary<CompressedMetaPointer, IKeyed> BuildSharedKeyMap(IEnumerable<Language> languages)
    {
        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap = [];

        foreach (IKeyed keyed in languages.SelectMany(l => M1Extensions.Descendants<IKeyed>(l)))
        {
            var metaPointer = keyed switch
            {
                LanguageEntity l => l.ToMetaPointer(),
                Feature feat => feat.ToMetaPointer(),
                EnumerationLiteral l => l.GetEnumeration().ToMetaPointer(),
                _ => throw new NotImplementedException(keyed.GetType().Name)
            };

            sharedKeyedMap[CompressedMetaPointer.Create(metaPointer, true)] = keyed;
        }

        return sharedKeyedMap;
    }

    public IDeltaEvent Map(IDeltaCommand deltaCommand) =>
        deltaCommand switch
        {
            AddProperty a => new PropertyAdded(a.Parent, a.Property, a.NewValue, NextSequence(), OriginCommands(a),
                null),
            DeleteProperty a => new PropertyDeleted(a.Parent, a.Property, null, NextSequence(), OriginCommands(a),
                null),
            ChangeProperty a => new PropertyChanged(a.Parent, a.Property, a.NewValue, null, NextSequence(),
                OriginCommands(a), null),
            AddChild a => new ChildAdded(a.Parent, a.Containment, a.Index, a.NewChild, NextSequence(),
                OriginCommands(a), null),
            DeleteChild a => new ChildDeleted(a.Parent, a.Containment, a.Index, new([]), NextSequence(),
                OriginCommands(a), null),
            ReplaceChild a => new ChildReplaced(a.Parent, a.Containment, a.Index, a.NewChild, new([]), NextSequence(),
                OriginCommands(a), null),
            MoveChildFromOtherContainment a => new ChildMovedFromOtherContainment(a.NewParent, a.NewContainment,
                a.NewIndex, a.MovedChild, GetParent(a.MovedChild), GetContainment(a.MovedChild), GetIndex(a.MovedChild),
                NextSequence(), OriginCommands(a), null),
            MoveChildFromOtherContainmentInSameParent a => new ChildMovedFromOtherContainmentInSameParent(
                a.NewContainment, a.NewIndex, a.MovedChild, GetParent(a.MovedChild), GetContainment(a.MovedChild),
                GetIndex(a.MovedChild), NextSequence(), OriginCommands(a), null),
            MoveChildInSameContainment a => new ChildMovedInSameContainment(a.NewIndex, a.MovedChild,
                GetParent(a.MovedChild), GetContainment(a.MovedChild), GetIndex(a.MovedChild), NextSequence(),
                OriginCommands(a), null),
            AddAnnotation a => new AnnotationAdded(a.Parent, a.Index, a.NewAnnotation, NextSequence(),
                OriginCommands(a), null),
            DeleteAnnotation a => new AnnotationDeleted(a.Parent, a.Index, new([]), NextSequence(), OriginCommands(a),
                null),
            ReplaceAnnotation a => new AnnotationReplaced(a.Parent, a.Index, a.NewAnnotation, new([]), NextSequence(),
                OriginCommands(a), null),
            MoveAnnotationFromOtherParent a => new AnnotationMovedFromOtherParent(a.NewParent, a.NewIndex,
                a.MovedAnnotation, GetParent(a.MovedAnnotation), GetAnnotationIndex(a.MovedAnnotation), NextSequence(),
                OriginCommands(a), null),
            MoveAnnotationInSameParent a => new AnnotationMovedInSameParent(a.NewIndex, a.MovedAnnotation,
                GetParent(a.MovedAnnotation), GetAnnotationIndex(a.MovedAnnotation), NextSequence(), OriginCommands(a),
                null),
            AddReference a => new ReferenceAdded(a.Parent, a.Reference, a.Index, a.NewTarget, NextSequence(),
                OriginCommands(a), null),
            DeleteReference a => new ReferenceDeleted(a.Parent, a.Reference, a.Index, new(), NextSequence(),
                OriginCommands(a), null),
            ChangeReference a => new ReferenceChanged(a.Parent, a.Reference, a.Index, a.NewTarget, new(),
                NextSequence(), OriginCommands(a), null),
        };

    private NodeId GetParent(NodeId childId)
    {
        var child = (IWritableNode)_sharedNodeMap[childId];
        return child.GetParent().GetId();
    }

    private MetaPointer GetContainment(NodeId childId)
    {
        var child = (IWritableNode)_sharedNodeMap[childId];
        var parent = (IWritableNode)child.GetParent();
        return parent.GetContainmentOf(child).ToMetaPointer();
    }

    private Int32 GetIndex(NodeId childId)
    {
        var child = (IWritableNode)_sharedNodeMap[childId];
        var parent = (IWritableNode)child.GetParent();
        var containment = parent.GetContainmentOf(child);
        return M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList().IndexOf(child);
    }

    private Int32 GetAnnotationIndex(NodeId annotationId)
    {
        var annotation = _sharedNodeMap[annotationId];
        var parent = annotation.GetParent();
        return parent.GetAnnotations().ToList().IndexOf(annotation);
    }


    private long _nextSequence = 0;

    private long NextSequence() =>
        _nextSequence++;

    private CommandSource[] OriginCommands(ISingleDeltaCommand a) =>
        [new CommandSource("myParticipation", a.CommandId)];
}