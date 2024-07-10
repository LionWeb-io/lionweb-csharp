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

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;

public abstract class SerializerBase
{
    protected SerializedLanguageReference SerializeLanguageReference(Language language) =>
        new() { Key = language.Key, Version = language.Version };

    protected SerializedProperty SerializePropertySetting(IReadableNode node, Property property)
    {
        var value = GetValueIfSet(node, property);

        return new SerializedProperty
        {
            Property = property.ToMetaPointer(),
            Value = property.Type switch
            {
                _ when value == null => null,
                PrimitiveType => AsString(value),
                Enumeration => (value as Enum).LionCoreKey(),
                _ => throw new ArgumentException($"unsupported property: {property}", nameof(property))
            }
        };
    }

    protected SerializedReferenceTarget SerializeReferenceTarget(IReadableNode target)
    {
        var resolveInfo = target switch
        {
            INamed namedTarget when namedTarget.CollectAllSetFeatures().Contains(BuiltInsLanguage.Instance.INamed_name)
                => namedTarget.Name,
            _ => null
        };
        return new() { Reference = target.GetId(), ResolveInfo = resolveInfo };
    }

    protected object? GetValueIfSet(IReadableNode node, Feature feature) =>
        node.CollectAllSetFeatures().Contains(feature) ? node.Get(feature) : null;

    /// <summary>
    /// Serializes the given <paramref name="value">runtime value</paramref> as a string,
    /// conforming to the LionWeb JSON serialization format.
    /// 
    /// <em>Note!</em> No exception is thrown when the given runtime value doesn't correspond to a primitive type defined here.
    /// Instead, the runtime value is simply coerced to a string using its <c>ToString</c> method.
    /// </summary>
    private string? AsString(object? value)
        => value switch
        {
            null => null,
            bool boolean => boolean ? "true" : "false",
            string @string => @string,
            _ => value.ToString() // Integer, Json (and anything else)
        };

    protected virtual void LogError(string message) =>
        Console.Error.WriteLine(message);

    protected SerializedNode SerializeSimpleNode(IReadableNode node) =>
        new()
        {
            Id = node.GetId(),
            Classifier = node.GetClassifier().ToMetaPointer(),
            Properties = node.GetClassifier().AllFeatures().OfType<Property>()
                .Select(property => SerializedPropertySetting(node, property)).ToArray(),
            Containments = node.GetClassifier().AllFeatures().OfType<Containment>()
                .Select<Containment, SerializedContainment>(containment =>
                    SerializedContainmentSetting(node, containment)).ToArray(),
            References = node.GetClassifier().AllFeatures().OfType<Reference>()
                .Select<Reference, SerializedReference>(reference => SerializedReferenceSetting(node, reference))
                .ToArray(),
            Annotations = node.GetAnnotations()
                .Select(annotation => annotation.GetId()).ToArray(),
            Parent = node.GetParent()?.GetId()
        };

    private SerializedContainment SerializedContainmentSetting(IReadableNode node, Containment containment)
    {
        var value = GetValueIfSet(node, containment);
        return new SerializedContainment
        {
            Containment = containment.ToMetaPointer(),
            Children = value != null
                ? containment.AsNodes<INode>(value).Select((child) => child.GetId()).ToArray()
                : []
        };
    }

    private SerializedReference SerializedReferenceSetting(IReadableNode node, Reference reference)
    {
        var value = GetValueIfSet(node, reference);
        return new()
        {
            Reference = reference.ToMetaPointer(),
            Targets = value != null
                ? reference.AsNodes<INode>(value).Select(SerializedReferenceTarget).ToArray()
                : []
        };
    }
}