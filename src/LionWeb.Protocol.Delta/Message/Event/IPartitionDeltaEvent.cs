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

namespace LionWeb.Protocol.Delta.Message.Event;

using Core.Serialization;
using System.Text;
using System.Text.Json.Serialization;

public interface IPartitionDeltaEvent : IDeltaEvent;

#region Nodes

public interface INodeEvent : IPartitionDeltaEvent;

public record ClassifierChanged(
    TargetNode Node,
    MetaPointer NewClassifier,
    MetaPointer OldClassifier,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), INodeEvent;

#endregion

public interface IFeatureEvent : IPartitionDeltaEvent
{
    TargetNode Parent { get; }
    
    [JsonIgnore]
    MetaPointer Feature { get; }
};

#region Properties

public interface IPropertyEvent : IFeatureEvent
{
    TargetNode Node { get; }

    /// <inheritdoc />
    [JsonIgnore]
    TargetNode IFeatureEvent.Parent => Node;

    MetaPointer Property { get; }

    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IFeatureEvent.Feature => Property;
};

public record PropertyAdded(
    TargetNode Node,
    MetaPointer Property,
    PropertyValue NewValue,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IPropertyEvent;

public record PropertyDeleted(
    TargetNode Node,
    MetaPointer Property,
    PropertyValue OldValue,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IPropertyEvent;

public record PropertyChanged(
    TargetNode Node,
    MetaPointer Property,
    PropertyValue NewValue,
    PropertyValue OldValue,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IPropertyEvent;

#endregion

#region Children

public interface IContainmentEvent : IFeatureEvent
{
    [JsonIgnore]
    MetaPointer Containment { get; }

    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IFeatureEvent.Feature => Containment;
};

public record ChildAdded(
    TargetNode Parent,
    DeltaSerializationChunk NewChild,
    MetaPointer Containment,
    Index Index,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IContainmentEvent;

public record ChildDeleted(
    TargetNode DeletedChild,
    TargetNode[] DeletedDescendants,
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    public virtual bool Equals(ChildDeleted? other)
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
               string.Equals(DeletedChild, other.DeletedChild, StringComparison.InvariantCulture) &&
               DeletedDescendants.ArrayEquals(other.DeletedDescendants) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture) &&
               Containment.Equals(other.Containment) &&
               Index == other.Index;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(DeletedChild, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(DeletedDescendants);
        hashCode.Add(Parent, StringComparer.InvariantCulture);
        hashCode.Add(Containment);
        hashCode.Add(Index);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(DeletedChild));
        builder.Append(" = ");
        builder.Append(DeletedChild);
        builder.Append(", ");

        builder.Append(nameof(DeletedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(DeletedDescendants);
        builder.Append(", ");

        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        builder.Append(", ");

        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        builder.Append(", ");

        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);

        return true;
    }
}

public record ChildReplaced(
    DeltaSerializationChunk NewChild,
    TargetNode ReplacedChild,
    TargetNode[] ReplacedDescendants,
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    public virtual bool Equals(ChildReplaced? other)
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
               NewChild.Equals(other.NewChild) &&
               string.Equals(ReplacedChild, other.ReplacedChild, StringComparison.InvariantCulture) &&
               ReplacedDescendants.ArrayEquals(other.ReplacedDescendants) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture) &&
               Containment.Equals(other.Containment) &&
               Index == other.Index;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(NewChild);
        hashCode.Add(ReplacedChild, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(ReplacedDescendants);
        hashCode.Add(Parent, StringComparer.InvariantCulture);
        hashCode.Add(Containment);
        hashCode.Add(Index);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(NewChild));
        builder.Append(" = ");
        builder.Append(NewChild);
        builder.Append(", ");

        builder.Append(nameof(ReplacedChild));
        builder.Append(" = ");
        builder.Append(ReplacedChild);
        builder.Append(", ");

        builder.Append(nameof(ReplacedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(ReplacedDescendants);
        builder.Append(", ");

        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        builder.Append(", ");

        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        builder.Append(", ");

        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);

        return true;
    }
}

public record ChildMovedFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode OldParent,
    MetaPointer OldContainment,
    Index OldIndex,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public TargetNode Parent => NewParent;

    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IContainmentEvent.Containment => NewContainment;
}

public record ChildMovedFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer OldContainment,
    Index OldIndex,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IContainmentEvent.Containment => NewContainment;
}

public record ChildMovedInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index OldIndex,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages),
    IContainmentEvent;

public record ChildMovedAndReplacedFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode OldParent,
    MetaPointer OldContainment,
    Index OldIndex,
    TargetNode ReplacedChild,
    TargetNode[] ReplacedDescendants,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public TargetNode Parent => NewParent;

    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IContainmentEvent.Containment => NewContainment;

    /// <inheritdoc />
    public virtual bool Equals(ChildMovedAndReplacedFromOtherContainment? other)
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
               string.Equals(NewParent, other.NewParent, StringComparison.InvariantCulture) &&
               NewContainment.Equals(other.NewContainment) &&
               NewIndex == other.NewIndex &&
               string.Equals(MovedChild, other.MovedChild, StringComparison.InvariantCulture) &&
               string.Equals(OldParent, other.OldParent, StringComparison.InvariantCulture) &&
               OldContainment.Equals(other.OldContainment) &&
               OldIndex == other.OldIndex &&
               string.Equals(ReplacedChild, other.ReplacedChild, StringComparison.InvariantCulture) &&
               ReplacedDescendants.ArrayEquals(other.ReplacedDescendants);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(NewParent, StringComparer.InvariantCulture);
        hashCode.Add(NewContainment);
        hashCode.Add(NewIndex);
        hashCode.Add(MovedChild, StringComparer.InvariantCulture);
        hashCode.Add(OldParent, StringComparer.InvariantCulture);
        hashCode.Add(OldContainment);
        hashCode.Add(OldIndex);
        hashCode.Add(ReplacedChild, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(ReplacedDescendants);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(NewParent));
        builder.Append(" = ");
        builder.Append(NewParent);
        builder.Append(", ");

        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        builder.Append(", ");

        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        builder.Append(", ");

        builder.Append(nameof(OldParent));
        builder.Append(" = ");
        builder.Append(OldParent);
        builder.Append(", ");

        builder.Append(nameof(OldContainment));
        builder.Append(" = ");
        builder.Append(OldContainment);
        builder.Append(", ");

        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        builder.Append(", ");

        builder.Append(nameof(ReplacedChild));
        builder.Append(" = ");
        builder.Append(ReplacedChild);
        builder.Append(", ");

        builder.Append(nameof(ReplacedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(ReplacedDescendants);

        return true;
    }
}

public record ChildMovedAndReplacedFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer OldContainment,
    Index OldIndex,
    TargetNode ReplacedChild,
    TargetNode[] ReplacedDescendants,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IContainmentEvent.Containment => NewContainment;

    /// <inheritdoc />
    public virtual bool Equals(ChildMovedAndReplacedFromOtherContainmentInSameParent? other)
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
               NewContainment.Equals(other.NewContainment) &&
               NewIndex == other.NewIndex &&
               string.Equals(MovedChild, other.MovedChild, StringComparison.InvariantCulture) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture) &&
               OldContainment.Equals(other.OldContainment) &&
               OldIndex == other.OldIndex &&
               string.Equals(ReplacedChild, other.ReplacedChild, StringComparison.InvariantCulture) &&
               ReplacedDescendants.ArrayEquals(other.ReplacedDescendants);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(NewContainment);
        hashCode.Add(NewIndex);
        hashCode.Add(MovedChild, StringComparer.InvariantCulture);
        hashCode.Add(Parent, StringComparer.InvariantCulture);
        hashCode.Add(OldContainment);
        hashCode.Add(OldIndex);
        hashCode.Add(ReplacedChild, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(ReplacedDescendants);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(NewContainment));
        builder.Append(" = ");
        builder.Append(NewContainment);
        builder.Append(", ");

        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        builder.Append(", ");

        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        builder.Append(", ");

        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        builder.Append(", ");

        builder.Append(nameof(OldContainment));
        builder.Append(" = ");
        builder.Append(OldContainment);
        builder.Append(", ");

        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        builder.Append(", ");

        builder.Append(nameof(ReplacedChild));
        builder.Append(" = ");
        builder.Append(ReplacedChild);
        builder.Append(", ");

        builder.Append(nameof(ReplacedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(ReplacedDescendants);

        return true;
    }
}

public record ChildMovedAndReplacedInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index OldIndex,
    TargetNode ReplacedChild,
    TargetNode[] ReplacedDescendants,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    public virtual bool Equals(ChildMovedAndReplacedInSameContainment? other)
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
               NewIndex == other.NewIndex &&
               string.Equals(MovedChild, other.MovedChild, StringComparison.InvariantCulture) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture) &&
               Containment.Equals(other.Containment) &&
               OldIndex == other.OldIndex &&
               string.Equals(ReplacedChild, other.ReplacedChild, StringComparison.InvariantCulture) &&
               ReplacedDescendants.ArrayEquals(other.ReplacedDescendants);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(NewIndex);
        hashCode.Add(MovedChild, StringComparer.InvariantCulture);
        hashCode.Add(Parent, StringComparer.InvariantCulture);
        hashCode.Add(Containment);
        hashCode.Add(OldIndex);
        hashCode.Add(ReplacedChild, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(ReplacedDescendants);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        builder.Append(", ");

        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        builder.Append(", ");

        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        builder.Append(", ");

        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        builder.Append(", ");

        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        builder.Append(", ");

        builder.Append(nameof(ReplacedChild));
        builder.Append(" = ");
        builder.Append(ReplacedChild);
        builder.Append(", ");

        builder.Append(nameof(ReplacedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(ReplacedDescendants);

        return true;
    }
}

#endregion

#region Annotations

public interface IAnnotationEvent : IPartitionDeltaEvent
{
    TargetNode Parent { get; }
};

public record AnnotationAdded(
    TargetNode Parent,
    DeltaSerializationChunk NewAnnotation,
    Index Index,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IAnnotationEvent;

public record AnnotationDeleted(
    TargetNode DeletedAnnotation,
    TargetNode[] DeletedDescendants,
    TargetNode Parent,
    Index Index,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IAnnotationEvent
{
    /// <inheritdoc />
    public virtual bool Equals(AnnotationDeleted? other)
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
               string.Equals(DeletedAnnotation, other.DeletedAnnotation, StringComparison.InvariantCulture) &&
               DeletedDescendants.ArrayEquals(other.DeletedDescendants) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture) &&
               Index == other.Index;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(DeletedAnnotation, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(DeletedDescendants);
        hashCode.Add(Parent, StringComparer.InvariantCulture);
        hashCode.Add(Index);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(DeletedAnnotation));
        builder.Append(" = ");
        builder.Append(DeletedAnnotation);
        builder.Append(", ");

        builder.Append(nameof(DeletedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(DeletedDescendants);
        builder.Append(", ");

        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        builder.Append(", ");

        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);

        return true;
    }
}

public record AnnotationReplaced(
    DeltaSerializationChunk NewAnnotation,
    TargetNode ReplacedAnnotation,
    TargetNode[] ReplacedDescendants,
    TargetNode Parent,
    Index Index,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IAnnotationEvent
{
    /// <inheritdoc />
    public virtual bool Equals(AnnotationReplaced? other)
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
               NewAnnotation.Equals(other.NewAnnotation) &&
               string.Equals(ReplacedAnnotation, other.ReplacedAnnotation, StringComparison.InvariantCulture) &&
               ReplacedDescendants.ArrayEquals(other.ReplacedDescendants) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture) &&
               Index == other.Index;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(NewAnnotation);
        hashCode.Add(ReplacedAnnotation, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(ReplacedDescendants);
        hashCode.Add(Parent, StringComparer.InvariantCulture);
        hashCode.Add(Index);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(ReplacedAnnotation));
        builder.Append(" = ");
        builder.Append(ReplacedAnnotation);
        builder.Append(", ");

        builder.Append(nameof(ReplacedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(ReplacedDescendants);
        builder.Append(", ");

        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        builder.Append(", ");

        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);

        return true;
    }
}

public record AnnotationMovedFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode OldParent,
    Index OldIndex,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IAnnotationEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    TargetNode IAnnotationEvent.Parent => NewParent;
}

public record AnnotationMovedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IAnnotationEvent;

public record AnnotationMovedAndReplacedFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode OldParent,
    Index OldIndex,
    TargetNode ReplacedAnnotation,
    TargetNode[] ReplacedDescendants,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IAnnotationEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    TargetNode IAnnotationEvent.Parent => NewParent;

    /// <inheritdoc />
    public virtual bool Equals(AnnotationMovedAndReplacedFromOtherParent? other)
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
               string.Equals(NewParent, other.NewParent, StringComparison.InvariantCulture) &&
               NewIndex == other.NewIndex &&
               string.Equals(MovedAnnotation, other.MovedAnnotation, StringComparison.InvariantCulture) &&
               string.Equals(OldParent, other.OldParent, StringComparison.InvariantCulture) &&
               OldIndex == other.OldIndex &&
               string.Equals(ReplacedAnnotation, other.ReplacedAnnotation, StringComparison.InvariantCulture) &&
               ReplacedDescendants.ArrayEquals(other.ReplacedDescendants);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(NewParent, StringComparer.InvariantCulture);
        hashCode.Add(NewIndex);
        hashCode.Add(MovedAnnotation, StringComparer.InvariantCulture);
        hashCode.Add(OldParent, StringComparer.InvariantCulture);
        hashCode.Add(OldIndex);
        hashCode.Add(ReplacedAnnotation, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(ReplacedDescendants);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(NewParent));
        builder.Append(" = ");
        builder.Append(NewParent);
        builder.Append(", ");

        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        builder.Append(", ");

        builder.Append(nameof(MovedAnnotation));
        builder.Append(" = ");
        builder.Append(MovedAnnotation);
        builder.Append(", ");

        builder.Append(nameof(OldParent));
        builder.Append(" = ");
        builder.Append(OldParent);
        builder.Append(", ");

        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        builder.Append(", ");

        builder.Append(nameof(ReplacedAnnotation));
        builder.Append(" = ");
        builder.Append(ReplacedAnnotation);
        builder.Append(", ");

        builder.Append(nameof(ReplacedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(ReplacedDescendants);

        return true;
    }
}

public record AnnotationMovedAndReplacedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    TargetNode ReplacedAnnotation,
    TargetNode[] ReplacedDescendants,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IAnnotationEvent
{
    /// <inheritdoc />
    public virtual bool Equals(AnnotationMovedAndReplacedInSameParent? other)
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
               NewIndex == other.NewIndex &&
               string.Equals(MovedAnnotation, other.MovedAnnotation, StringComparison.InvariantCulture) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture) &&
               OldIndex == other.OldIndex &&
               string.Equals(ReplacedAnnotation, other.ReplacedAnnotation, StringComparison.InvariantCulture) &&
               ReplacedDescendants.ArrayEquals(other.ReplacedDescendants);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(NewIndex);
        hashCode.Add(MovedAnnotation, StringComparer.InvariantCulture);
        hashCode.Add(Parent, StringComparer.InvariantCulture);
        hashCode.Add(OldIndex);
        hashCode.Add(ReplacedAnnotation, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(ReplacedDescendants);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        builder.Append(", ");

        builder.Append(nameof(MovedAnnotation));
        builder.Append(" = ");
        builder.Append(MovedAnnotation);
        builder.Append(", ");

        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        builder.Append(", ");

        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        builder.Append(", ");

        builder.Append(nameof(ReplacedAnnotation));
        builder.Append(" = ");
        builder.Append(ReplacedAnnotation);
        builder.Append(", ");

        builder.Append(nameof(ReplacedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(ReplacedDescendants);

        return true;
    }
}

#endregion

#region References

public interface IReferenceEvent : IFeatureEvent
{
    MetaPointer Reference { get; }

    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IFeatureEvent.Feature => Reference;
};

public record ReferenceAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? NewTarget,
    ResolveInfo? NewResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? DeletedTarget,
    ResolveInfo? DeletedResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? NewTarget,
    ResolveInfo? NewResolveInfo,
    TargetNode? OldTarget,
    ResolveInfo? OldResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record EntryMovedFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    TargetNode? Target,
    ResolveInfo? ResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public TargetNode Parent => NewParent;

    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    TargetNode? Target,
    ResolveInfo? ResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index OldIndex,
    Index NewIndex,
    TargetNode? Target,
    ResolveInfo? ResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record EntryMovedAndReplacedFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    TargetNode? ReplacedTarget,
    ResolveInfo? ReplacedResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public TargetNode Parent => NewParent;

    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedAndReplacedFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    MetaPointer OldReference,
    Index OldIndex,
    TargetNode? ReplacedTarget,
    ResolveInfo? ReplacedResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedAndReplacedInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    Index OldIndex,
    TargetNode? ReplacedTarget,
    ResolveInfo? ReplacedResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceResolveInfoAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode Target,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceResolveInfoDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode Target,
    ResolveInfo DeletedResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceResolveInfoChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode? Target,
    ResolveInfo ReplacedResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceTargetAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo ResolveInfo,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceTargetDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo ResolveInfo,
    TargetNode DeletedTarget,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceTargetChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo? ResolveInfo,
    TargetNode ReplacedTarget,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IReferenceEvent;

#endregion

