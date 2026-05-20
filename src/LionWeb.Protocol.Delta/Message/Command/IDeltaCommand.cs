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

namespace LionWeb.Protocol.Delta.Message.Command;

using Core.Serialization;
using System.Text;
using System.Text.Json.Serialization;

/// <remarks>
/// IMPORTANT: Make sure to update attributes on <see cref="IDeltaContent"/> in lockstep.
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
[JsonPolymorphic(TypeDiscriminatorPropertyName = "messageKind", IgnoreUnrecognizedTypeDiscriminators = false, UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
public interface IDeltaCommand : IDeltaContent
{
    CommandId? CommandId { get; }
}

public abstract record DeltaCommandBase(
    CommandId? CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaContentBase(AdditionalInfos), IDeltaCommand
{
    /// <inheritdoc />
    [JsonIgnore]
    public override string Id => CommandId;

    /// <inheritdoc />
    public virtual bool Equals(DeltaCommandBase? other)
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
               string.Equals(CommandId, other.CommandId, StringComparison.InvariantCulture);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(CommandId, StringComparer.InvariantCulture);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(CommandId));
        builder.Append(" = ");
        builder.Append(CommandId);

        return true;
    }
}

public record CompositeCommand(
    IDeltaCommand[] Parts,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IDeltaCommand
{
    /// <inheritdoc />
    public virtual bool Equals(CompositeCommand? other)
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
               Parts.ArrayEquals(other.Parts);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.ArrayHashCode(Parts);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");
        builder.Append(nameof(Parts));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Parts);

        return true;
    }
}