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

// ReSharper disable CoVariantArrayConversion

namespace LionWeb.Protocol.Delta.Message.Event;

using Core.Serialization;
using System.Text;
using System.Text.Json.Serialization;

public record CommandSource(ParticipationId ParticipationId, CommandId CommandId)
{
    /// <summary>
    /// Represents this command source as NodeId-compatible string.
    /// </summary>
    /// <returns></returns>
    public string AsId() =>
        ParticipationId + "__" + CommandId;
};

/// <remarks>
/// IMPORTANT: Make sure to update attributes on <see cref="IDeltaContent"/> in lockstep.
/// </remarks> 
#region Event

[JsonDerivedType(typeof(CompositeEvent), nameof(CompositeEvent))]
[JsonDerivedType(typeof(ErrorEvent), nameof(ErrorEvent))]
[JsonDerivedType(typeof(NoOpEvent), nameof(NoOpEvent))]

#region Forest

[JsonDerivedType(typeof(PartitionAdded), nameof(PartitionAdded))]
[JsonDerivedType(typeof(PartitionDeleted), nameof(PartitionDeleted))]

#endregion

#region Partition

#region Annotation

[JsonDerivedType(typeof(AnnotationAdded), nameof(AnnotationAdded))]
[JsonDerivedType(typeof(AnnotationDeleted), nameof(AnnotationDeleted))]
[JsonDerivedType(typeof(AnnotationMovedAndReplacedFromOtherParent), nameof(AnnotationMovedAndReplacedFromOtherParent))]
[JsonDerivedType(typeof(AnnotationMovedAndReplacedInSameParent), nameof(AnnotationMovedAndReplacedInSameParent))]
[JsonDerivedType(typeof(AnnotationMovedFromOtherParent), nameof(AnnotationMovedFromOtherParent))]
[JsonDerivedType(typeof(AnnotationMovedInSameParent), nameof(AnnotationMovedInSameParent))]
[JsonDerivedType(typeof(AnnotationReplaced), nameof(AnnotationReplaced))]

#endregion

#region Feature

#region Containment

[JsonDerivedType(typeof(ChildAdded), nameof(ChildAdded))]
[JsonDerivedType(typeof(ChildDeleted), nameof(ChildDeleted))]
[JsonDerivedType(typeof(ChildMovedAndReplacedFromOtherContainment), nameof(ChildMovedAndReplacedFromOtherContainment))]
[JsonDerivedType(typeof(ChildMovedAndReplacedFromOtherContainmentInSameParent), nameof(ChildMovedAndReplacedFromOtherContainmentInSameParent))]
[JsonDerivedType(typeof(ChildMovedAndReplacedInSameContainment), nameof(ChildMovedAndReplacedInSameContainment))]
[JsonDerivedType(typeof(ChildMovedFromOtherContainment), nameof(ChildMovedFromOtherContainment))]
[JsonDerivedType(typeof(ChildMovedFromOtherContainmentInSameParent), nameof(ChildMovedFromOtherContainmentInSameParent))]
[JsonDerivedType(typeof(ChildMovedInSameContainment), nameof(ChildMovedInSameContainment))]
[JsonDerivedType(typeof(ChildReplaced), nameof(ChildReplaced))]

#endregion

#region Property

[JsonDerivedType(typeof(PropertyAdded), nameof(PropertyAdded))]
[JsonDerivedType(typeof(PropertyChanged), nameof(PropertyChanged))]
[JsonDerivedType(typeof(PropertyDeleted), nameof(PropertyDeleted))]

#endregion

#region Reference

[JsonDerivedType(typeof(ReferenceAdded), nameof(ReferenceAdded))]
[JsonDerivedType(typeof(ReferenceChanged), nameof(ReferenceChanged))]
[JsonDerivedType(typeof(ReferenceDeleted), nameof(ReferenceDeleted))]

#endregion

#endregion

#region Node

[JsonDerivedType(typeof(ClassifierChanged), nameof(ClassifierChanged))]

#endregion

#endregion

#endregion
[JsonPolymorphic(TypeDiscriminatorPropertyName = "messageKind", IgnoreUnrecognizedTypeDiscriminators = false, UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
public interface IDeltaEvent : IDeltaContent
{
    public const EventSequenceNumber DefaultEventSequenceNumber = -1;

    EventSequenceNumber SequenceNumber { get; set; }

    CommandSource[]? OriginCommands { get; }

    [JsonIgnore] HashSet<TargetNode> AffectedNodes { get; }
}

public abstract record DeltaEventBase(
    CommandSource[]? OriginCommands,
    AdditionalInfo[]? AdditionalInfos) : DeltaContentBase(AdditionalInfos), IDeltaEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public override string Id => string.Join("___", OriginCommands.Select(x => x.AsId()));

    /// <inheritdoc />
    public virtual EventSequenceNumber SequenceNumber { get; set; } = IDeltaEvent.DefaultEventSequenceNumber;

    /// <inheritdoc />
    public virtual CommandSource[]? OriginCommands { get; init; } = OriginCommands;

    /// <inheritdoc />
    [JsonIgnore]
    public abstract HashSet<TargetNode> AffectedNodes { get; }

    /// <inheritdoc />
    public virtual bool Equals(DeltaEventBase? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) &&
               OriginCommands.ArrayEquals(other.OriginCommands);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.ArrayHashCode(OriginCommands);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(SequenceNumber));
        builder.Append(" = ");
        builder.Append(SequenceNumber);
        builder.Append(", ");

        builder.Append(nameof(OriginCommands));
        builder.Append(" = ");
        builder.ArrayPrintMembers(OriginCommands);

        return true;
    }
}

#region Miscellaneous

public record CompositeEvent : DeltaEventBase, IDeltaEvent
{
    public CompositeEvent(IDeltaEvent[] Parts,
        CommandSource[]? OriginCommands,
        AdditionalInfo[]? AdditionalInfos) : base(OriginCommands, AdditionalInfos)
    {
        this.Parts = Parts;
    }

    /// <inheritdoc />
    [JsonIgnore]
    public override string Id => base.Id;

    /// <inheritdoc />
    [JsonIgnore]
    public override HashSet<TargetNode> AffectedNodes => Parts.SelectMany(p => p.AffectedNodes).ToHashSet();

    public IDeltaEvent[] Parts { get; init; }

    /// <inheritdoc />
    [JsonIgnore]
    public override EventSequenceNumber SequenceNumber { get; set; } = IDeltaEvent.DefaultEventSequenceNumber;

    /// <inheritdoc />
    [JsonIgnore]
    public override CommandSource[]? OriginCommands => base.OriginCommands;

    /// <inheritdoc />
    public virtual bool Equals(CompositeEvent? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) &&
               Parts.ArrayEquals(other.Parts) &&
               SequenceNumber == other.SequenceNumber;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.ArrayHashCode(Parts);
        hashCode.Add(SequenceNumber);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        // do NOT call base.PrintMembers(), as we omit OriginCommands

        builder.Append(nameof(AdditionalInfos));
        builder.Append(" = ");
        builder.ArrayPrintMembers(AdditionalInfos);

        builder.Append(", ");

        builder.Append(nameof(SequenceNumber));
        builder.Append(" = ");
        builder.Append(SequenceNumber);
        builder.Append(", ");

        builder.Append(nameof(Parts));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Parts);

        return true;
    }
}

public record NoOpEvent(
    CommandSource[]? OriginCommands,
    AdditionalInfo[]? AdditionalInfos) : DeltaEventBase(OriginCommands, AdditionalInfos), IDeltaEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public override HashSet<TargetNode> AffectedNodes => [];
}

public record ErrorEvent(
    ErrorCode ErrorCode,
    string Message,
    CommandSource[]? OriginCommands,
    AdditionalInfo[]? AdditionalInfos) : DeltaEventBase(OriginCommands, AdditionalInfos), IDeltaEvent, IDeltaError
{
    /// <inheritdoc />
    [JsonIgnore]
    public override HashSet<TargetNode> AffectedNodes => [];
}

#endregion