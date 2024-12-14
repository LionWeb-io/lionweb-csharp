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

namespace LionWeb.Core;

using M1;
using M2;
using M3;
using Serialization;

/// <summary>
/// Common base type for all LionWeb exceptions.
/// </summary>
public abstract class LionWebExceptionBase : ArgumentException
{
    /// <inheritdoc />
    protected LionWebExceptionBase(string? message) : base(message)
    {
    }

    /// <inheritdoc />
    protected LionWebExceptionBase(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <inheritdoc />
    protected LionWebExceptionBase(string? message, string? paramName) : base(message, paramName)
    {
    }

    /// <inheritdoc />
    protected LionWebExceptionBase(string? message, string? paramName, Exception? innerException) : base(message,
        paramName, innerException)
    {
    }
}

/// <summary>
/// Trying to create a <see cref="ReadableNodeBase{T}"/> with <see cref="IReadableNode.GetId">invalid id</see>.
/// </summary>
/// <param name="id">Invalid id.</param>
public class InvalidIdException(string? id) : LionWebExceptionBase($"Invalid node id: {id}", "id");

/// <summary>
/// Trying to set a <see cref="Feature"/> to an unsupported value.
/// </summary>
/// <param name="feature">Feature trying to set.</param>
/// <param name="value">Invalid value.</param>
public class InvalidValueException(Feature? feature, object? value) : LionWebExceptionBase(feature switch
{
    null => $"annotations cannot have value '{value ?? "null"}'",
    not null =>
        $"{feature.GetClassifier().Name} {feature.Name} of type {feature.GetFeatureType().Name} cannot have value '{value ?? "null"}'"
});

/// <summary>
/// Trying to retrieve a <see cref="Feature"/> that hasn't been set yet.
/// </summary>
/// <param name="feature">Feature trying to retrieve.</param>
public class UnsetFeatureException(Feature feature)
    : LionWebExceptionBase($"Required {feature.GetClassifier().Name} {feature.Name} is not set");

/// <summary>
/// Trying to operate on a <see cref="Feature"/> unknown to a <see cref="Classifier"/>.
/// </summary>
public class UnknownFeatureException : LionWebExceptionBase
{
    /// <param name="classifier">Classifier trying to use.</param>
    /// <param name="feature">Feature unknown to <paramref name="classifier"/>.</param>
    /// <param name="message">Optional additional message.</param>
    public UnknownFeatureException(Classifier classifier, Feature? feature, string? message = null) : base(
        $"{message}Classifier {classifier.Name} does not know feature {feature?.Name}")
    {
    }

    /// <param name="classifier">Classifier trying to use.</param>
    /// <param name="metaPointer"><see cref="Feature"/> unknown to <paramref name="classifier"/>.</param>
    /// <param name="message">Optional additional message.</param>
    public UnknownFeatureException(Classifier classifier, MetaPointer metaPointer, string? message = null) : base(
        $"{message}Classifier {classifier.Name} does not know feature {metaPointer}")
    {
    }

    /// <param name="classifier">Classifier trying to use.</param>
    /// <param name="compressedMetaPointer"><see cref="Feature"/> unknown to <paramref name="classifier"/>.</param>
    /// <param name="message">Optional additional message.</param>
    public UnknownFeatureException(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        string? message = null) : base(
        $"{message}Classifier {classifier.Name} does not know feature {compressedMetaPointer}")
    {
    }
}

/// <summary>
/// Trying to operate on an unsupported <see cref="Classifier"/>.
/// Should not happen, guards against cases where somebody tries to extend LionWeb M3.
/// </summary>
public class UnsupportedClassifierException : LionWebExceptionBase
{
    /// <param name="classifier">Unsupported classifier.</param>
    public UnsupportedClassifierException(Classifier classifier) : base($"Classifier {classifier.Name} not supported")
    {
    }

    /// <param name="metaPointer">Unsupported <see cref="Classifier"/>.</param>
    /// <param name="message">Optional additional message.</param>
    public UnsupportedClassifierException(MetaPointer metaPointer, string? message = null) : base(
        $"{message}Classifier {metaPointer} not supported")
    {
    }

    /// <param name="compressedMetaPointer">Unsupported <see cref="Classifier"/>.</param>
    /// <param name="message">Optional additional message.</param>
    public UnsupportedClassifierException(CompressedMetaPointer compressedMetaPointer, string? message = null) : base(
        $"{message}Classifier {compressedMetaPointer} not supported")
    {
    }
}

/// <summary>
/// Trying to operate on an <see cref="EnumerationLiteral"/> that belongs to an unsupported <see cref="Enumeration"/>.
/// Might happen if a non-LionWeb enumeration is used with LionWeb nodes.
/// </summary>
/// <param name="literal">Unsupported enumeration literal.</param>
public class UnsupportedEnumerationLiteralException(EnumerationLiteral literal)
    : LionWebExceptionBase($"EnumerationLiteral {literal.Name} not supported");

/// <summary>
/// Trying to operate on an object that's not an <see cref="INode"/>.
/// </summary>
/// <param name="node">Unsupported object.</param>
/// <param name="paramName">Origin of <paramref name="node"/>.</param>
public class UnsupportedNodeTypeException(object? node, string? paramName)
    : LionWebExceptionBase($"Unsupported node type: {node?.GetType().ToString() ?? "null"}", paramName);

/// <summary>
/// Trying to operate on <paramref name="node"/> in a way violates its <see cref="IReadableNode.GetParent">parent's</see> conditions
/// (e.g. <paramref name="node"/> has no parent, or the parent's containment of <paramref name="node"/> doesn't fit preconditions). 
/// </summary>
/// <param name="node">Node trying to operate on.</param>
/// <param name="message">Description of failed operation.</param>
public class TreeShapeException(IReadableNode node, string message)
    : LionWebExceptionBase($"{node.GetId()}: {message}");

/// <summary>
/// Trying to use an unsupported version of LionWeb standard.
/// </summary>
/// <seealso cref="LionWebVersions"/>
public class UnsupportedVersionException : LionWebExceptionBase
{
    /// <inheritdoc cref="UnsupportedVersionException" />
    public UnsupportedVersionException(LionWebVersions version) : base($"Unsupported LionWeb version: {version}")
    {
    }

    /// <inheritdoc cref="UnsupportedVersionException" />
    public UnsupportedVersionException(string version) : base($"Unsupported LionWeb version: {version}")
    {
    }
}

public class VersionMismatchException(string? versionA, string versionB, string? message = null)
    : LionWebExceptionBase($"Mismatched LionWeb versions: {versionA} vs. {versionB}" + (message != null ? $": {message}" : ""));
