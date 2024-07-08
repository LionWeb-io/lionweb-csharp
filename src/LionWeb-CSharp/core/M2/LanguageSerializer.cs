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

namespace LionWeb.Core.M2;

using M1;
using M3;
using Serialization;

/// <summary>
/// A serializer that serializes <see cref="Language"/> definitions.
/// The generic serializer isn't aware of the LionCore M3-types (and their idiosyncrasies),
/// so that can't be used.
/// </summary>
public class LanguageSerializer : SerializerBase
{
    private readonly IEnumerable<Language> _languages;

    public LanguageSerializer(IEnumerable<Language> languages)
    {
        _languages = languages;
    }

    /// <summary>
    /// Serializes the given <paramref name="languages">language( definition)s</paramref>.
    /// </summary>
    /// 
    /// <returns>The serialization of the given language definitions as a <see cref="SerializationChunk"/>.</returns>
    public static SerializationChunk Serialize(params Language[] languages) =>
        new LanguageSerializer(languages).Serialize();

    public SerializationChunk Serialize() =>
        new()
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                SerializeLanguageReference(M3Language.Instance)
            ],
            Nodes = _languages.SelectMany(language => M1Extensions.Descendants<IKeyed>(language, true))
                .Select(SerializeNode).ToArray()
        };

    private SerializedNode SerializeNode(IKeyed node)
    {
        var metaConcept = node switch
        {
            M3Language => M3Language.Instance.Language,
            M3Concept => M3Language.Instance.Concept,
            M3Interface => M3Language.Instance.Interface,
            M3Property => M3Language.Instance.Property,
            M3Containment => M3Language.Instance.Containment,
            M3Reference => M3Language.Instance.Reference,
            var k => k.GetClassifier()
        };

        var serializedNode = new SerializedNode
        {
            Id = node.GetId(),
            Classifier = metaConcept.ToMetaPointer(),
            Properties = [],
            Containments = [],
            References = [],
            Annotations = [],
            Parent = node.GetParent()?.GetId()
        };

        switch (node)
        {
            case Annotation annotation:
                SerializeAnnotation(serializedNode, metaConcept, annotation);
                break;

            case Concept concept:
                SerializeConcept(serializedNode, metaConcept, concept);
                break;

            case Enumeration enumeration:
                SerializeEnumeration(serializedNode, metaConcept, enumeration);
                break;

            case EnumerationLiteral enumerationLiteral:
                SerializeEnumerationLiteral(serializedNode, metaConcept, enumerationLiteral);
                break;

            case Interface @interface:
                SerializeInterface(serializedNode, metaConcept, @interface);
                break;

            case Language language:
                SerializeLanguage(serializedNode, metaConcept, language);
                break;

            case Link link:
                SerializeLink(serializedNode, metaConcept, link);
                break;

            case PrimitiveType primitiveType:
                SerializePrimitiveType(serializedNode, metaConcept, primitiveType);
                break;

            case Property property:
                SerializeProperty(serializedNode, metaConcept, property);
                break;

            default:
                logError($"serialization of {metaConcept.Name} not (yet) implemented");
                break;
        }

        return serializedNode;
    }

    private void SerializeAnnotation(SerializedNode serializedNode, Classifier metaConcept, Annotation annotation)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", annotation),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", annotation)
        ];
        serializedNode.Containments =
        [
            SerializedContainmentSettings(metaConcept, "Classifier-features", annotation.Features)
        ];
        serializedNode.References =
        [
            SerializedReferenceSetting(metaConcept, "Annotation-extends", annotation.Extends),
            SerializedReferenceSettings(metaConcept, "Annotation-implements", annotation.Implements),
            SerializedReferenceSetting(metaConcept, "Annotation-annotates", annotation.Annotates)
        ];
    }

    private void SerializeConcept(SerializedNode serializedNode, Classifier metaConcept, Concept concept)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", concept),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", concept),
            SerializedPropertySetting(metaConcept, "Concept-abstract", concept),
            SerializedPropertySetting(metaConcept, "Concept-partition", concept)
        ];
        serializedNode.Containments =
        [
            SerializedContainmentSettings(metaConcept, "Classifier-features", concept.Features)
        ];
        serializedNode.References =
        [
            SerializedReferenceSetting(metaConcept, "Concept-extends", concept.Extends),
            SerializedReferenceSettings(metaConcept, "Concept-implements", concept.Implements)
        ];
    }

    private void SerializeEnumeration(SerializedNode serializedNode, Classifier metaConcept, Enumeration enumeration)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", enumeration),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", enumeration)
        ];
        serializedNode.Containments =
        [
            SerializedContainmentSettings(metaConcept, "Enumeration-literals", enumeration.Literals)
        ];
    }

    private void SerializeEnumerationLiteral(SerializedNode serializedNode, Classifier metaConcept,
        EnumerationLiteral enumerationLiteral)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", enumerationLiteral),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", enumerationLiteral)
        ];
    }

    private void SerializeInterface(SerializedNode serializedNode, Classifier metaConcept, Interface @interface)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", @interface),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", @interface)
        ];
        serializedNode.Containments =
        [
            SerializedContainmentSettings(metaConcept, "Classifier-features", @interface.Features)
        ];
        serializedNode.References =
        [
            SerializedReferenceSettings(metaConcept, "Interface-extends", @interface.Extends)
        ];
    }

    private void SerializeLanguage(SerializedNode serializedNode, Classifier metaConcept, Language language)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", language),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", language),
            SerializedPropertySetting(metaConcept, "Language-version", language)
        ];
        serializedNode.Containments =
        [
            SerializedContainmentSettings(metaConcept, "Language-entities", language.Entities)
        ];
        serializedNode.References =
        [
            SerializedReferenceSettings(metaConcept, "Language-dependsOn", language.DependsOn)
        ];
    }

    private void SerializeLink(SerializedNode serializedNode, Classifier metaConcept, Link link)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", link),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", link),
            SerializedPropertySetting(metaConcept, "Feature-optional", link),
            SerializedPropertySetting(metaConcept, "Link-multiple", link)
        ];
        serializedNode.References =
        [
            SerializedReferenceSetting(metaConcept, "Link-type", link.Type)
        ];
    }

    private void SerializePrimitiveType(SerializedNode serializedNode, Classifier metaConcept,
        PrimitiveType primitiveType)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", primitiveType),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", primitiveType)
        ];
    }

    private void SerializeProperty(SerializedNode serializedNode, Classifier metaConcept, Property property)
    {
        serializedNode.Properties =
        [
            SerializedPropertySetting(metaConcept, "IKeyed-key", property),
            SerializedPropertySetting(metaConcept, "LionCore-builtins-INamed-name", property),
            SerializedPropertySetting(metaConcept, "Feature-optional", property)
        ];
        serializedNode.References =
        [
            SerializedReferenceSetting(metaConcept, "Property-type", property.Type)
        ];
    }

    private SerializedProperty SerializedPropertySetting(Classifier metaConcept, string key, IReadableNode node)
        => SerializePropertySetting(node, (Property)metaConcept.FeatureByKey(key));

    private SerializedContainment SerializedContainmentSettings(Classifier metaConcept, string key,
        IEnumerable<IReadableNode> children)
        => new()
        {
            Containment = metaConcept.FeatureByKey(key).ToMetaPointer(),
            Children = children.Select(child => child.GetId()).ToArray()
        };

    private SerializedReference SerializedReferenceSetting(Classifier metaConcept, string key, IReadableNode? target)
        => new()
        {
            Reference = metaConcept.FeatureByKey(key).ToMetaPointer(),
            Targets = target == null
                ? []
                : [SerializeReferenceTarget(target)]
        };

    private SerializedReference SerializedReferenceSettings(Classifier metaConcept, string key,
        IEnumerable<IReadableNode> targets)
        => new()
        {
            Reference = metaConcept.FeatureByKey(key).ToMetaPointer(),
            Targets = targets.Select(SerializeReferenceTarget).ToArray()
        };
}