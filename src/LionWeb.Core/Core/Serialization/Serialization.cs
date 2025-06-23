// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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
namespace LionWeb.Core.Serialization;

using Delta;
using System.Text;

/// <summary>
/// This type, together with all the types in this file, represent data structures
/// to capture a parsing of a LionWeb serialization chunk in JSON format.
/// </summary>
public record SerializationChunk
{
    /// <inheritdoc />
    public virtual bool Equals(SerializationChunk? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(SerializationFormatVersion, other.SerializationFormatVersion,
                   StringComparison.InvariantCulture) &&
               Languages.ArrayEquals(other.Languages) &&
               Nodes.ArrayEquals(other.Nodes);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        
        hashCode.Add(SerializationFormatVersion, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(Languages);
        hashCode.ArrayHashCode(Nodes);

        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(SerializationFormatVersion));
        builder.Append(" = ");
        builder.Append(SerializationFormatVersion);
        builder.Append(", ");

        builder.Append(nameof(Languages));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Languages);
        builder.Append(", ");

        builder.Append(nameof(Nodes));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Nodes);

        return true;
    }

    public required string SerializationFormatVersion { get; init; }
    public required SerializedLanguageReference[] Languages { get; init; }
    public required SerializedNode[] Nodes { get; init; }
}

public record SerializedLanguageReference
{
    public required string Key { get; set; }
    public required string Version { get; set; }
}

public record SerializedNode
{
    /// <inheritdoc />
    public virtual bool Equals(SerializedNode? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Id, other.Id, StringComparison.InvariantCulture) &&
               Classifier.Equals(other.Classifier) &&
               Properties.ArrayEquals(other.Properties) &&
               Containments.ArrayEquals(other.Containments) &&
               References.ArrayEquals(other.References) &&
               Annotations.ArrayEquals(other.Annotations) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        
        hashCode.Add(Id, StringComparer.InvariantCulture);
        hashCode.Add(Classifier);
        hashCode.ArrayHashCode(Properties);
        hashCode.ArrayHashCode(Containments);
        hashCode.ArrayHashCode(References);
        hashCode.ArrayHashCode(Annotations);
        hashCode.Add(Parent, StringComparer.InvariantCulture);
        
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Id));
        builder.Append(" = ");
        builder.Append(Id);
        builder.Append(", ");

        builder.Append(nameof(Classifier));
        builder.Append(" = ");
        builder.Append(Classifier);
        builder.Append(", ");

        builder.Append(nameof(Properties));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Properties);
        builder.Append(", ");

        builder.Append(nameof(Containments));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Containments);
        builder.Append(", ");

        builder.Append(nameof(References));
        builder.Append(" = ");
        builder.ArrayPrintMembers(References);
        builder.Append(", ");

        builder.Append(nameof(Annotations));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Annotations);
        builder.Append(", ");

        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);

        return true;
    }

    public required string Id { get; init; }
    public required MetaPointer Classifier { get; init; }
    public required SerializedProperty[] Properties { get; init; }
    public required SerializedContainment[] Containments { get; init; }
    public required SerializedReference[] References { get; init; }
    public required string[] Annotations { get; init; }
    public string? Parent { get; init; }
}

public record MetaPointer(string Language, string Version, string Key);

public record SerializedProperty
{
    public required MetaPointer Property { get; init; }
    public string? Value { get; init; }
}

public record SerializedContainment
{
    /// <inheritdoc />
    public virtual bool Equals(SerializedContainment? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Containment.Equals(other.Containment) &&
               Children.ArrayEquals(other.Children);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        
        hashCode.Add(Containment);
        hashCode.ArrayHashCode(Children);

        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        builder.Append(", ");

        builder.Append(nameof(Children));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Children);
        
        return true;
    }

    public required MetaPointer Containment { get; init; }
    public required string[] Children { get; init; }
}

public record SerializedReferenceTarget
{
    public string? ResolveInfo { get; init; }
    public string? Reference { get; init; }
}

public record SerializedReference
{
    /// <inheritdoc />
    public virtual bool Equals(SerializedReference? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Reference.Equals(other.Reference) &&
               Targets.ArrayEquals(other.Targets);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        
        hashCode.Add(Reference);
        hashCode.ArrayHashCode(Targets);

        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        builder.Append(", ");

        builder.Append(nameof(Targets));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Targets);
        
        return true;
    }

    public required MetaPointer Reference { get; init; }
    public required SerializedReferenceTarget[] Targets { get; init; }
}