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

namespace LionWeb.Protocol.Delta.Message;

using Command;
using Core.Serialization;
using Event;
using Query;
using System.Text;
using System.Text.Json.Serialization;

public sealed class DeltaProtocolVersion
{
    private readonly string _version;

    private DeltaProtocolVersion(string version)
    {
        _version = version;
    }

    public static DeltaProtocolVersion? v2025_1 = new("2025.1");

    public string Version => _version;

    public override string ToString() => _version;
}

public record DeltaSerializationChunk(SerializedNode[] Nodes)
{
    /// <inheritdoc />
    public virtual bool Equals(DeltaSerializationChunk? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Nodes.ArrayEquals(other.Nodes);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.ArrayHashCode(Nodes);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Nodes));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Nodes);

        return true;
    }
}

public record AdditionalInfo(MessageKind Kind, string Message, AdditionalInfoData[]? Data)
{
    /// <inheritdoc />
    public virtual bool Equals(AdditionalInfo? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Kind, other.Kind, StringComparison.InvariantCulture) &&
               string.Equals(Message, other.Message, StringComparison.InvariantCulture) &&
               Data.ArrayEquals(other.Data);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Kind, StringComparer.InvariantCulture);
        hashCode.Add(Message, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(Data);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Kind));
        builder.Append(" = ");
        builder.Append(Kind);
        builder.Append(", ");

        builder.Append(nameof(Message));
        builder.Append(" = ");
        builder.Append(Message);
        builder.Append(", ");

        builder.Append(nameof(Data));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Data);

        return true;
    }
}

public record AdditionalInfoData(AdditionalInfoDataKey Key, string Value)
{
    /// <inheritdoc />
    public virtual bool Equals(AdditionalInfoData? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Key, other.Key, StringComparison.InvariantCulture) &&
               string.Equals(Value, other.Value, StringComparison.InvariantCulture);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Key, StringComparer.InvariantCulture);
        hashCode.Add(Value, StringComparer.InvariantCulture);
        return hashCode.ToHashCode();
    }
}

public interface IDeltaError
{
    ErrorCode ErrorCode { get; }
    string Message { get; }
}

/// <remarks>
/// IMPORTANT: Make sure to update attributes on <see cref="IDeltaCommand"/> and <see cref="IDeltaEvent"/> in lockstep.
/// </remarks> 
#region Command

[JsonDerivedType(typeof(CompositeCommand), nameof(CompositeCommand))]

#region Forest

[JsonDerivedType(typeof(AddPartition), nameof(AddPartition))]
[JsonDerivedType(typeof(DeletePartition), nameof(DeletePartition))]

#endregion

#region Partition

#region Annotation

[JsonDerivedType(typeof(AddAnnotation), nameof(AddAnnotation))]
[JsonDerivedType(typeof(DeleteAnnotation), nameof(DeleteAnnotation))]
[JsonDerivedType(typeof(MoveAndReplaceAnnotationFromOtherParent), nameof(MoveAndReplaceAnnotationFromOtherParent))]
[JsonDerivedType(typeof(MoveAndReplaceAnnotationInSameParent), nameof(MoveAndReplaceAnnotationInSameParent))]
[JsonDerivedType(typeof(MoveAnnotationFromOtherParent), nameof(MoveAnnotationFromOtherParent))]
[JsonDerivedType(typeof(MoveAnnotationInSameParent), nameof(MoveAnnotationInSameParent))]
[JsonDerivedType(typeof(ReplaceAnnotation), nameof(ReplaceAnnotation))]

#endregion

#region Feature

#region Containment

[JsonDerivedType(typeof(AddChild), nameof(AddChild))]
[JsonDerivedType(typeof(DeleteChild), nameof(DeleteChild))]
[JsonDerivedType(typeof(MoveAndReplaceChildFromOtherContainment), nameof(MoveAndReplaceChildFromOtherContainment))]
[JsonDerivedType(typeof(MoveAndReplaceChildFromOtherContainmentInSameParent), nameof(MoveAndReplaceChildFromOtherContainmentInSameParent))]
[JsonDerivedType(typeof(MoveAndReplaceChildInSameContainment), nameof(MoveAndReplaceChildInSameContainment))]
[JsonDerivedType(typeof(MoveChildFromOtherContainment), nameof(MoveChildFromOtherContainment))]
[JsonDerivedType(typeof(MoveChildFromOtherContainmentInSameParent), nameof(MoveChildFromOtherContainmentInSameParent))]
[JsonDerivedType(typeof(MoveChildInSameContainment), nameof(MoveChildInSameContainment))]
[JsonDerivedType(typeof(ReplaceChild), nameof(ReplaceChild))]

#endregion

#region Property

[JsonDerivedType(typeof(AddProperty), nameof(AddProperty))]
[JsonDerivedType(typeof(ChangeProperty), nameof(ChangeProperty))]
[JsonDerivedType(typeof(DeleteProperty), nameof(DeleteProperty))]

#endregion

#region Reference

[JsonDerivedType(typeof(AddReference), nameof(AddReference))]
[JsonDerivedType(typeof(ChangeReference), nameof(ChangeReference))]
[JsonDerivedType(typeof(DeleteReference), nameof(DeleteReference))]

#endregion

#endregion

#region Node

[JsonDerivedType(typeof(ChangeClassifier), nameof(ChangeClassifier))]

#endregion

#endregion

#endregion

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

#region Query

#region Miscellaneous

[JsonDerivedType(typeof(GetAvailableIdsRequest), nameof(GetAvailableIdsRequest))]
[JsonDerivedType(typeof(GetAvailableIdsResponse), nameof(GetAvailableIdsResponse))]
[JsonDerivedType(typeof(ListPartitionsRequest), nameof(ListPartitionsRequest))]
[JsonDerivedType(typeof(ListPartitionsResponse), nameof(ListPartitionsResponse))]

#endregion

#region Participation

[JsonDerivedType(typeof(ReconnectRequest), nameof(ReconnectRequest))]
[JsonDerivedType(typeof(ReconnectResponse), nameof(ReconnectResponse))]
[JsonDerivedType(typeof(SignOffRequest), nameof(SignOffRequest))]
[JsonDerivedType(typeof(SignOffResponse), nameof(SignOffResponse))]
[JsonDerivedType(typeof(SignOnRequest), nameof(SignOnRequest))]
[JsonDerivedType(typeof(SignOnResponse), nameof(SignOnResponse))]

#endregion

#region Subscription

[JsonDerivedType(typeof(SubscribeToChangingPartitionsRequest), nameof(SubscribeToChangingPartitionsRequest))]
[JsonDerivedType(typeof(SubscribeToChangingPartitionsResponse), nameof(SubscribeToChangingPartitionsResponse))]
[JsonDerivedType(typeof(SubscribeToPartitionContentsRequest), nameof(SubscribeToPartitionContentsRequest))]
[JsonDerivedType(typeof(SubscribeToPartitionContentsResponse), nameof(SubscribeToPartitionContentsResponse))]
[JsonDerivedType(typeof(UnsubscribeFromPartitionContentsRequest), nameof(UnsubscribeFromPartitionContentsRequest))]
[JsonDerivedType(typeof(UnsubscribeFromPartitionContentsResponse), nameof(UnsubscribeFromPartitionContentsResponse))]

#endregion

#endregion

[JsonPolymorphic(TypeDiscriminatorPropertyName = "messageKind", IgnoreUnrecognizedTypeDiscriminators = false, UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
public interface IDeltaContent
{
    AdditionalInfo[]? AdditionalInfos { get; }

    [JsonIgnore]
    ParticipationId InternalParticipationId { get; set; }

    [JsonIgnore]
    public bool RequiresParticipationId => true;

    [JsonIgnore]
    string Id { get; }
}

public abstract record DeltaContentBase(AdditionalInfo[]? AdditionalInfos) : IDeltaContent
{
    [JsonIgnore]
    public ParticipationId InternalParticipationId { get; set; }

    [JsonIgnore]
    public abstract string Id { get; }

    public virtual AdditionalInfo[]? AdditionalInfos { get; init; } = AdditionalInfos;

    /// <inheritdoc />
    public virtual bool Equals(DeltaContentBase? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return AdditionalInfos.ArrayEquals(other.AdditionalInfos);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.ArrayHashCode(AdditionalInfos);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(AdditionalInfos));
        builder.Append(" = ");
        builder.ArrayPrintMembers(AdditionalInfos);

        return true;
    }
}