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
[Obsolete("Not needed anymore")]
public abstract class LanguageSerializer : SerializerBase
{
    /// <summary>
    /// Serializes the given <paramref name="languages">language( definition)s</paramref>.
    /// </summary>
    /// 
    /// <returns>The serialization of the given language definitions as a <see cref="SerializationChunk"/>.</returns>
    public static SerializationChunk Serialize(params Language[] languages) =>
        new Serializer().SerializeToChunk(languages);

    public override IEnumerable<SerializedNode> Serialize(IEnumerable<IReadableNode> allNodes) =>
        allNodes.Select(SerializeNode);

    private SerializedNode SerializeNode(IReadableNode node) => node switch
    {
        Annotation annotation => SerializeAnnotation(annotation),
        Concept concept => SerializeConcept(concept),
        Enumeration enumeration => SerializeEnumeration(enumeration),
        EnumerationLiteral enumerationLiteral => SerializeEnumerationLiteral(enumerationLiteral),
        Interface @interface => SerializeInterface(@interface),
        Language language => SerializeLanguage(language),
        Link link => SerializeLink(link),
        PrimitiveType primitiveType => SerializePrimitiveType(primitiveType),
        Property property => SerializeProperty(property),
        _ => SerializeSimpleNode(node)
    };

    private SerializedNode SerializeAnnotation(Annotation annotation) =>
        new()
        {
            Id = annotation.GetId(),
            Classifier = M3Language.Instance.Annotation.ToMetaPointer(),
            Annotations = [],
            Parent = annotation.GetParent()?.GetId(),
            Properties =
            [
                SerializeProperty(annotation, M3Language.Instance.IKeyed_key),
                SerializeProperty(annotation, BuiltInsLanguage.Instance.INamed_name)
            ],
            Containments =
            [
                SerializeContainment(annotation.Features, M3Language.Instance.Classifier_features)
            ],
            References =
            [
                SerializeReference([annotation.Extends], M3Language.Instance.Annotation_extends),
                SerializeReference(annotation.Implements, M3Language.Instance.Annotation_implements),
                SerializeReference([annotation.Annotates], M3Language.Instance.Annotation_annotates)
            ]
        };

    private SerializedNode SerializeConcept(Concept concept) =>
        new()
        {
            Id = concept.GetId(),
            Classifier = M3Language.Instance.Concept.ToMetaPointer(),
            Parent = concept.GetParent()?.GetId(),
            Annotations = [],
            Properties =
            [
                SerializeProperty(concept, M3Language.Instance.IKeyed_key),
                SerializeProperty(concept, BuiltInsLanguage.Instance.INamed_name),
                SerializeProperty(concept, M3Language.Instance.Concept_abstract),
                SerializeProperty(concept, M3Language.Instance.Concept_partition)
            ],
            Containments =
            [
                SerializeContainment(concept.Features, M3Language.Instance.Classifier_features)
            ],
            References =
            [
                SerializeReference([concept.Extends], M3Language.Instance.Concept_extends),
                SerializeReference(concept.Implements, M3Language.Instance.Concept_implements)
            ]
        };

    private SerializedNode SerializeEnumeration(Enumeration enumeration) =>
        new()
        {
            Id = enumeration.GetId(),
            Classifier = M3Language.Instance.Enumeration.ToMetaPointer(),
            Parent = enumeration.GetParent()?.GetId(),
            Annotations = [],
            Properties =
            [
                SerializeProperty(enumeration, M3Language.Instance.IKeyed_key),
                SerializeProperty(enumeration, BuiltInsLanguage.Instance.INamed_name)
            ],
            Containments =
            [
                SerializeContainment(enumeration.Literals, M3Language.Instance.Enumeration_literals)
            ],
            References = []
        };

    private SerializedNode SerializeEnumerationLiteral(EnumerationLiteral enumerationLiteral) =>
        new()
        {
            Id = enumerationLiteral.GetId(),
            Classifier = M3Language.Instance.EnumerationLiteral.ToMetaPointer(),
            Parent = enumerationLiteral.GetParent()?.GetId(),
            Annotations = [],
            Properties =
            [
                SerializeProperty(enumerationLiteral, M3Language.Instance.IKeyed_key),
                SerializeProperty(enumerationLiteral, BuiltInsLanguage.Instance.INamed_name)
            ],
            Containments = [],
            References = []
        };

    private SerializedNode SerializeInterface(Interface @interface) =>
        new()
        {
            Id = @interface.GetId(),
            Classifier = M3Language.Instance.Interface.ToMetaPointer(),
            Parent = @interface.GetParent()?.GetId(),
            Annotations = [],
            Properties =
            [
                SerializeProperty(@interface, M3Language.Instance.IKeyed_key),
                SerializeProperty(@interface, BuiltInsLanguage.Instance.INamed_name)
            ],
            Containments =
            [
                SerializeContainment(@interface.Features, M3Language.Instance.Classifier_features)
            ],
            References =
            [
                SerializeReference(@interface.Extends, M3Language.Instance.Interface_extends)
            ]
        };

    private SerializedNode SerializeLanguage(Language language) =>
        new()
        {
            Id = language.GetId(),
            Classifier = M3Language.Instance.Language.ToMetaPointer(),
            Parent = language.GetParent()?.GetId(),
            Annotations = [],
            Properties =
            [
                SerializeProperty(language, M3Language.Instance.IKeyed_key),
                SerializeProperty(language, BuiltInsLanguage.Instance.INamed_name),
                SerializeProperty(language, M3Language.Instance.Language_version)
            ],
            Containments =
            [
                SerializeContainment(language.Entities, M3Language.Instance.Language_entities)
            ],
            References =
            [
                SerializeReference(language.DependsOn, M3Language.Instance.Language_dependsOn)
            ]
        };

    private SerializedNode SerializeLink(Link link)
    {
        var metaConcept = link switch
        {
            Containment => M3Language.Instance.Containment,
            Reference => M3Language.Instance.Reference
        };
        return new SerializedNode
        {
            Id = link.GetId(),
            Classifier = metaConcept.ToMetaPointer(),
            Parent = link.GetParent()?.GetId(),
            Annotations = [],
            Properties =
            [
                SerializeProperty(link, M3Language.Instance.IKeyed_key),
                SerializeProperty(link, BuiltInsLanguage.Instance.INamed_name),
                SerializeProperty(link, M3Language.Instance.Feature_optional),
                SerializeProperty(link, M3Language.Instance.Link_multiple)
            ],
            Containments = [],
            References =
            [
                SerializeReference([link.Type], M3Language.Instance.Link_type)
            ]
        };
    }

    private SerializedNode SerializePrimitiveType(PrimitiveType primitiveType) =>
        new()
        {
            Id = primitiveType.GetId(),
            Classifier = M3Language.Instance.PrimitiveType.ToMetaPointer(),
            Parent = primitiveType.GetParent()
                ?.GetId(),
            Annotations = [],
            Properties =
            [
                SerializeProperty(primitiveType, M3Language.Instance.IKeyed_key),
                SerializeProperty(primitiveType, BuiltInsLanguage.Instance.INamed_name)
            ],
            Containments = [],
            References = []
        };

    private SerializedNode SerializeProperty(Property property) =>
        new()
        {
            Id = property.GetId(),
            Classifier = M3Language.Instance.Property.ToMetaPointer(),
            Parent = property.GetParent()?.GetId(),
            Annotations = [],
            Properties =
            [
                SerializeProperty(property, M3Language.Instance.IKeyed_key),
                SerializeProperty(property, BuiltInsLanguage.Instance.INamed_name),
                SerializeProperty(property, M3Language.Instance.Feature_optional)
            ],
            Containments = [],
            References =
            [
                SerializeReference([property.Type], M3Language.Instance.Property_type)
            ]
        };
}