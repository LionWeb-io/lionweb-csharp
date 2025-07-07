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

namespace LionWeb.Protocol.Delta;

using Command;
using Core;
using Core.M1;
using Core.M2;
using Core.M3;
using Core.Serialization;
using Event;

public class DeltaCommandToDeltaEventMapper
{
    private readonly Dictionary<NodeId, IReadableNode> _sharedNodeMap;
    private readonly ParticipationId _participationId;

    public DeltaCommandToDeltaEventMapper(ParticipationId participationId, Dictionary<NodeId, IReadableNode> sharedNodeMap)
    {
        _sharedNodeMap = sharedNodeMap;
        _participationId = participationId;
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
                Field f => f.ToMetaPointer(),
                _ => throw new NotImplementedException(keyed.GetType().Name)
            };

            sharedKeyedMap[CompressedMetaPointer.Create(metaPointer, true)] = keyed;
        }

        return sharedKeyedMap;
    }

    public IDeltaEvent Map(IDeltaCommand deltaCommand) =>
        deltaCommand switch
        {
            AddProperty a => new PropertyAdded(a.Node, a.Property, a.NewValue, OriginCommands(a), NextSequence(), []),
            DeleteProperty a => new PropertyDeleted(a.Node, a.Property, null, OriginCommands(a), NextSequence(), []),
            ChangeProperty a => new PropertyChanged(a.Node, a.Property, a.NewValue, null, OriginCommands(a), NextSequence(), []),
            AddChild a => new ChildAdded(a.Parent, a.NewChild, a.Containment, a.Index, OriginCommands(a), NextSequence(), []),
            DeleteChild a => new ChildDeleted(a.DeletedChild, [], a.Parent, a.Containment, a.Index, OriginCommands(a), NextSequence(), []),
            ReplaceChild a => new ChildReplaced(a.NewChild, a.ReplacedChild, [], a.Parent, a.Containment, a.Index , OriginCommands(a), NextSequence(), []),
            MoveChildFromOtherContainment a => new ChildMovedFromOtherContainment(a.NewParent, a.NewContainment, a.NewIndex, a.MovedChild, GetParent(a.MovedChild), GetContainment(a.MovedChild), GetIndex(a.MovedChild), OriginCommands(a), NextSequence(), []),
            MoveChildFromOtherContainmentInSameParent a => new ChildMovedFromOtherContainmentInSameParent(a.NewContainment, a.NewIndex, a.MovedChild, GetParent(a.MovedChild), GetContainment(a.MovedChild), GetIndex(a.MovedChild), OriginCommands(a), NextSequence(), []),
            MoveChildInSameContainment a => new ChildMovedInSameContainment(a.NewIndex, a.MovedChild, GetParent(a.MovedChild), GetContainment(a.MovedChild), GetIndex(a.MovedChild), OriginCommands(a), NextSequence(), []),
            AddAnnotation a => new AnnotationAdded(a.Parent, a.NewAnnotation, a.Index, OriginCommands(a), NextSequence(), []),
            DeleteAnnotation a => new AnnotationDeleted(a.DeletedAnnotation, [], a.Parent, a.Index, OriginCommands(a), NextSequence(), []),
            ReplaceAnnotation a => new AnnotationReplaced(a.NewAnnotation,a.ReplacedAnnotation, [], a.Parent, a.Index, OriginCommands(a), NextSequence(), []),
            MoveAnnotationFromOtherParent a => new AnnotationMovedFromOtherParent(a.NewParent, a.NewIndex, a.MovedAnnotation, GetParent(a.MovedAnnotation), GetAnnotationIndex(a.MovedAnnotation), OriginCommands(a), NextSequence(), []),
            MoveAnnotationInSameParent a => new AnnotationMovedInSameParent(a.NewIndex, a.MovedAnnotation, GetParent(a.MovedAnnotation), GetAnnotationIndex(a.MovedAnnotation), OriginCommands(a), NextSequence(), []),
            AddReference a => new ReferenceAdded(a.Parent, a.Reference, a.Index, a.NewTarget, a.NewResolveInfo, OriginCommands(a), NextSequence(), []),
            DeleteReference a => new ReferenceDeleted(a.Parent, a.Reference, a.Index, a.DeletedTarget, a.DeletedResolveInfo, OriginCommands(a), NextSequence(), []),
            ChangeReference a => new ReferenceChanged(a.Parent, a.Reference, a.Index, a.NewTarget, a.NewResolveInfo, a.OldTarget, a.OldResolveInfo, OriginCommands(a), NextSequence(), []),
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

    private CommandSource[] OriginCommands(IDeltaCommand a) =>
        [new CommandSource(_participationId, a.CommandId)];
}