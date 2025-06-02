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

namespace LionWeb.Core.Serialization;

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
               Languages.SequenceEqual(other.Languages) &&
               Nodes.SequenceEqual(other.Nodes);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(SerializationFormatVersion, StringComparer.InvariantCulture);
        foreach (var language in Languages)
        {
            hashCode.Add(language);
        }

        foreach (var node in Nodes)
        {
            hashCode.Add(node);
        }

        return hashCode.ToHashCode();
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
               Properties.SequenceEqual(other.Properties) &&
               Containments.SequenceEqual(other.Containments) &&
               References.SequenceEqual(other.References) &&
               Annotations.SequenceEqual(other.Annotations) &&
               string.Equals(Parent, other.Parent, StringComparison.InvariantCulture);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id, StringComparer.InvariantCulture);
        hashCode.Add(Classifier);
        foreach (var property in Properties)
        {
            hashCode.Add(property);
        }

        foreach (var containment in Containments)
        {
            hashCode.Add(containment);
        }

        foreach (var reference in References)
        {
            hashCode.Add(reference);
        }

        foreach (var annotation in Annotations)
        {
            hashCode.Add(annotation);
        }

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
        builder.Append(" = [");
        bool firstProperty = true;
        foreach (var property in Properties)
        {
            if (!firstProperty)
            {
                builder.Append(", ");
            }
            firstProperty = false;
            builder.Append(property);
        }
        builder.Append("], ");

        builder.Append(nameof(Containments));
        builder.Append(" = [");
        bool firstContainment = true;
        foreach (var containment in Containments)
        {
            if (!firstContainment)
            {
                builder.Append(", ");
            }
            firstContainment = false;
            builder.Append(containment);
        }
        builder.Append("], ");

        builder.Append(nameof(References));
        builder.Append(" = [");
        bool firstReference = true;
        foreach (var reference in References)
        {
            if (!firstReference)
            {
                builder.Append(", ");
            }
            firstReference = false;
            builder.Append(reference);
        }
        builder.Append("], ");

        builder.Append(nameof(Annotations));
        builder.Append(" = [");
        bool firstAnnotation = true;
        foreach (var annotation in Annotations)
        {
            if (!firstAnnotation)
            {
                builder.Append(", ");
            }
            firstAnnotation = false;
            builder.Append(annotation);
        }
        builder.Append("], ");

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
               Children.SequenceEqual(other.Children);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Containment);
        foreach (var child in Children)
        {
            hashCode.Add(child);
        }

        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        builder.Append(", ");

        builder.Append(nameof(Children));
        builder.Append(" = [");
        bool first = true;
        foreach (var child in Children)
        {
            if (!first)
            {
                builder.Append(", ");
            }
            first = false;
            builder.Append(child);
        }
        builder.Append(']');
        
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

        return Reference.Equals(other.Reference) && Targets.SequenceEqual(other.Targets);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Reference);
        foreach (var target in Targets)
        {
            hashCode.Add(target);
        }

        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        builder.Append(", ");

        builder.Append(nameof(Targets));
        builder.Append(" = [");
        bool first = true;
        foreach (var target in Targets)
        {
            if (!first)
            {
                builder.Append(", ");
            }
            first = false;
            builder.Append(target);
        }
        builder.Append(']');
        
        return true;
    }

    public required MetaPointer Reference { get; init; }
    public required SerializedReferenceTarget[] Targets { get; init; }
}