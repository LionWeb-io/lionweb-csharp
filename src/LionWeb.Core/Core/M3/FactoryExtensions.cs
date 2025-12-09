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

namespace LionWeb.Core.M3;

using M2;

/// <summary>
/// Extensions methods that provide factory methods on M3 types whose instances are always contained.
/// </summary>
public static class FactoryExtensions
{
    /// <inheritdoc cref="Annotation"/>
    public static DynamicAnnotation Annotation(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var annotation = new DynamicAnnotation(id, language.LionWebVersion, language) { Key = key, Name = name };
        language.AddEntitiesRaw(annotation);
        return annotation;
    }

    /// <inheritdoc cref="Concept"/>
    public static DynamicConcept Concept(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var concept = new DynamicConcept(id, language.LionWebVersion, language) { Key = key, Name = name };
        language.AddEntitiesRaw(concept);
        return concept;
    }

    /// <inheritdoc cref="Containment"/>
    public static DynamicContainment Containment(this DynamicClassifier classifier, NodeId id, MetaPointerKey key, string name)
    {
        var containment = new DynamicContainment(id, classifier.GetLanguage().LionWebVersion, classifier) { Key = key, Name = name };
        classifier.AddFeaturesRaw(containment);
        return containment;
    }

    /// <inheritdoc cref="Enumeration"/>
    public static DynamicEnumeration Enumeration(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var enumeration = new DynamicEnumeration(id, language.LionWebVersion, language) { Key = key, Name = name };
        language.AddEntitiesRaw(enumeration);
        return enumeration;
    }

    /// <inheritdoc cref="EnumerationLiteral"/>
    public static DynamicEnumerationLiteral EnumerationLiteral(this DynamicEnumeration enumeration, NodeId id,
        MetaPointerKey key, string name)
    {
        var enumerationLiteral = new DynamicEnumerationLiteral(id, enumeration.GetLanguage().LionWebVersion, enumeration) { Key = key, Name = name };
        enumeration.AddLiteralsRaw(enumerationLiteral);
        return enumerationLiteral;
    }

    /// <inheritdoc cref="Field"/>
    public static DynamicField Field(this DynamicStructuredDataType structuredDataType, NodeId id,
        MetaPointerKey key, string name)
    {
        var field = new DynamicField(id, structuredDataType.GetLanguage().LionWebVersion, structuredDataType) { Key = key, Name = name };
        structuredDataType.AddFieldsRaw(field);
        return field;
    }

    /// <inheritdoc cref="Field.Type"/>
    public static DynamicField OfType(this DynamicField field, Datatype type)
    {
        field.Type = type;
        return field;
    }

    /// <inheritdoc cref="Interface"/>
    public static DynamicInterface Interface(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var @interface = new DynamicInterface(id, language.LionWebVersion, language) { Key = key, Name = name };
        language.AddEntitiesRaw(@interface);
        return @interface;
    }

    /// <inheritdoc cref="PrimitiveType"/>
    public static DynamicPrimitiveType PrimitiveType(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var primitiveType = new DynamicPrimitiveType(id, language.LionWebVersion, language) { Key = key, Name = name };
        language.AddEntitiesRaw(primitiveType);
        return primitiveType;
    }

    /// <inheritdoc cref="Property"/>
    public static DynamicProperty Property(this DynamicClassifier classifier, NodeId id, MetaPointerKey key, string name)
    {
        var property = new DynamicProperty(id, classifier.GetLanguage().LionWebVersion, classifier) { Key = key, Name = name };
        classifier.AddFeaturesRaw(property);
        return property;
    }

    /// <inheritdoc cref="Reference"/>
    public static DynamicReference Reference(this DynamicClassifier classifier, NodeId id, MetaPointerKey key, string name)
    {
        var reference = new DynamicReference(id, classifier.GetLanguage().LionWebVersion, classifier) { Key = key, Name = name };
        classifier.AddFeaturesRaw(reference);
        return reference;
    }

    /// <inheritdoc cref="StructuredDataType"/>
    public static DynamicStructuredDataType StructuredDataType(this DynamicLanguage language, NodeId id, MetaPointerKey key,
        string name)
    {
        var structuredDataType = new DynamicStructuredDataType(id, language.LionWebVersion, language) { Key = key, Name = name };
        language.AddEntitiesRaw(structuredDataType);
        return structuredDataType;
    }
}