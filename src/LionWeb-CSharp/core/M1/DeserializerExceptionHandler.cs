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

public class DeserializerExceptionHandler : IDeserializerHandler
{
    /// <inheritdoc />
    public virtual Classifier? UnknownClassifier(string id, CompressedMetaPointer metaPointer) =>
        throw new DeserializerException($"On node with id={id}:");
        // throw new UnsupportedClassifierException(metaPointer, $"On node with id={id}:");

    /// <inheritdoc />
    public virtual Feature? UnknownFeature(CompressedMetaPointer metaPointer, INode node) =>
        throw new DeserializerException($"On node with id={node.GetId()}:");
        // throw new UnknownFeatureException(classifier, metaPointer, $"On node with id={id}:");

    /// <inheritdoc />
    public virtual INode? UnknownParent(CompressedId parentId, INode node)
    {
        Console.WriteLine(
            $"On node with id={node.GetId()}: couldn't find specified parent - leaving this node orphaned.");
        return null;
    }

    /// <inheritdoc />
    public virtual INode? UnknownChild(CompressedId childId, INode node)
    {
        Console.WriteLine($"On node with id={node.GetId()}: couldn't find child with id={childId} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public virtual IReadableNode? UnknownReference(CompressedId targetId, string? resolveInfo, INode node)
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
}