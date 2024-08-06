// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.M1;

using M3;
using Serialization;

public interface IDeserializerHandler
{
    Classifier? UnknownClassifier(string id, MetaPointer metaPointer);
    Feature? UnknownFeature(Classifier classifier, CompressedMetaPointer compressedMetaPointer, IReadableNode node);
    INode? UnknownParent(CompressedId parentId, INode node);
    INode? UnknownChild(CompressedId childId, IWritableNode node);
    IReadableNode? UnknownReference(CompressedId targetId, string? resolveInfo, IWritableNode node);
    INode? UnknownAnnotation(CompressedId annotationId, INode node);
    INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node);
    Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key);
    object? UnknownDatatype(string nodeId, Property property, string? value);
    bool SkipDeserializingDependentNode(string id);

    TFeature? InvalidFeature<TFeature>(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        IReadableNode node)
        where TFeature : class, Feature;

    void InvalidContainment(IReadableNode node);
    void InvalidReference(IReadableNode node);
    IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId);
}

public class DeserializerExceptionHandler : IDeserializerHandler
{
    /// <inheritdoc />
    public virtual Classifier? UnknownClassifier(string id, MetaPointer metaPointer) =>
        throw new UnsupportedClassifierException(metaPointer, $"On node with id={id}: ");

    /// <inheritdoc />
    public virtual Feature? UnknownFeature(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        IReadableNode node)
    {
        var message = $"On node with id={node.GetId()}:";
        if (compressedMetaPointer.Original is { } metaPointer)
            throw new UnknownFeatureException(classifier, metaPointer, message);
        throw new UnknownFeatureException(classifier, (Feature?)null, message);
    }

    /// <inheritdoc />
    public TFeature? InvalidFeature<TFeature>(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        IReadableNode node) where TFeature : class, Feature
    {
        var message = $"On node with id={node.GetId()}:";
        if (compressedMetaPointer.Original is { } metaPointer)
            throw new UnknownFeatureException(classifier, metaPointer, message);
        throw new UnknownFeatureException(classifier, (Feature?)null, message);
    }

    /// <inheritdoc />
    public void InvalidContainment(IReadableNode node) =>
        throw new UnsupportedClassifierException(node.GetClassifier().ToMetaPointer(),
            $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public void InvalidReference(IReadableNode node) =>
        throw new UnsupportedClassifierException(node.GetClassifier().ToMetaPointer(),
            $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual INode? UnknownParent(CompressedId parentId, INode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: couldn't find specified parent with id={parentId}");

    /// <inheritdoc />
    public virtual INode? UnknownChild(CompressedId childId, IWritableNode node) =>
        throw new DeserializerException($"On node with id={node.GetId()}: couldn't find child with id={childId}");

    /// <inheritdoc />
    public virtual IReadableNode? UnknownReference(CompressedId targetId, string? resolveInfo, IWritableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: couldn't find reference with id={targetId}");

    /// <inheritdoc />
    public virtual INode? UnknownAnnotation(CompressedId annotationId, INode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: couldn't find annotation with id={annotationId}");

    /// <inheritdoc />
    public INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node) =>
        throw new DeserializerException(
            $"On node with id={node?.GetId()}: unsuitable annotation {annotation}");

    /// <inheritdoc />
    public IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId) =>
        throw new DeserializerException($"Cannot attach annotation {annotation} to its parent with id={parentId}.");

    /// <inheritdoc />
    public Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key) =>
        throw new DeserializerException(
            $"On node with id={nodeId}: unknown enumeration literal for enumeration {enumeration} with key {key}");

    /// <inheritdoc />
    public object? UnknownDatatype(string nodeId, Property property, string? value) =>
        throw new DeserializerException(
            $"On node with id={nodeId}: unknown property type {property.Type} with value {value}");

    /// <inheritdoc />
    public bool SkipDeserializingDependentNode(string id) =>
        throw new DeserializerException(
            $"Skip deserializing {id} because dependentLanguages contains node with same id");
}

internal class DeserializerException(string? message) : LionWebExceptionBase(message);

public class DeserializerIgnoringHandler : IDeserializerHandler
{
    /// <inheritdoc />
    public virtual Classifier? UnknownClassifier(string id, MetaPointer metaPointer)
    {
        Console.WriteLine(
            $"On node with id={id}: couldn't find specified classifier {metaPointer} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public virtual Feature? UnknownFeature(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        IReadableNode node)
    {
        Console.WriteLine(
            $"On node with id={node.GetId()}: couldn't find specified feature {compressedMetaPointer} - leaving this feature unset.");
        return null;
    }

    /// <inheritdoc />
    public void InvalidContainment(IReadableNode node) =>
        Console.WriteLine($"installing containments in node of meta-concept {node.GetType().Name} not implemented");

    public void InvalidReference(IReadableNode node) =>
        Console.WriteLine($"installing references in node of meta-concept {node.GetType().Name} not implemented");

    /// <inheritdoc />
    public TFeature? InvalidFeature<TFeature>(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        IReadableNode node) where TFeature : class, Feature
    {
        Console.WriteLine(
            $"On node with id={node.GetId()}: wrong type of feature {compressedMetaPointer} - leaving this feature unset.");
        return null;
    }

    /// <inheritdoc />
    public virtual INode? UnknownParent(CompressedId parentId, INode node)
    {
        Console.WriteLine(
            $"On node with id={node.GetId()}: couldn't find specified parent - leaving this node orphaned.");
        return null;
    }

    /// <inheritdoc />
    public virtual INode? UnknownChild(CompressedId childId, IWritableNode node)
    {
        Console.WriteLine($"On node with id={node.GetId()}: couldn't find child with id={childId} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public virtual IReadableNode? UnknownReference(CompressedId targetId, string? resolveInfo, IWritableNode node)
    {
        Console.WriteLine(
            $"On node with id={node.GetId()}: couldn't find reference with id={targetId} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public virtual INode? UnknownAnnotation(CompressedId annotationId, INode node)
    {
        Console.WriteLine(
            $"On node with id={node.GetId()}: couldn't find annotation with id={annotationId} - skipping.");
        return null;
    }

    public IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId)
    {
        Console.WriteLine($"Cannot attach annotation {annotation} to its parent with id={parentId}.");
        return null;
    }

    /// <inheritdoc />
    public INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node)
    {
        Console.WriteLine(
            $"On node with id={node?.GetId()}: unsuitable annotation {annotation} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key)
    {
        Console.WriteLine(
            $"On node with id={nodeId}: unknown enumeration literal for enumeration {enumeration} with key {key} - skipping");
        return null;
    }

    /// <inheritdoc />
    public object? UnknownDatatype(string nodeId, Property property, string? value)
    {
        Console.WriteLine(
            $"On node with id={nodeId}: unknown datatype {property.Type} with value {value} - skipping");
        return null;
    }

    /// <inheritdoc />
    public bool SkipDeserializingDependentNode(string id)
    {
        Console.WriteLine(
            $"Skip deserializing {id} because dependent nodes contains node with same id");
        return true;
    }
}