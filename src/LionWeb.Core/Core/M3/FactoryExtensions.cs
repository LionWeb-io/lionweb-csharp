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

/// <summary>
/// Extensions methods that provide factory methods on M3 types whose instances are always contained.
/// </summary>
public static class FactoryExtensions
{
    /// <inheritdoc cref="Annotation"/>
    public static DynamicAnnotation Annotation(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var annotation = new DynamicAnnotation(id, language) { Key = key, Name = name };
        language.AddEntities([annotation]);
        return annotation;
    }

    /// <inheritdoc cref="Concept"/>
    public static DynamicConcept Concept(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var concept = new DynamicConcept(id, language) { Key = key, Name = name };
        language.AddEntities([concept]);
        return concept;
    }

    /// <inheritdoc cref="Containment"/>
    public static DynamicContainment Containment(this DynamicClassifier classifier, NodeId id, MetaPointerKey key, string name)
    {
        var containment = new DynamicContainment(id, classifier) { Key = key, Name = name };
        classifier.AddFeatures([containment]);
        return containment;
    }

    /// <inheritdoc cref="Enumeration"/>
    public static DynamicEnumeration Enumeration(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var enumeration = new DynamicEnumeration(id, language) { Key = key, Name = name };
        language.AddEntities([enumeration]);
        return enumeration;
    }

    /// <inheritdoc cref="EnumerationLiteral"/>
    public static DynamicEnumerationLiteral EnumerationLiteral(this DynamicEnumeration enumeration, NodeId id,
        MetaPointerKey key, string name)
    {
        var enumerationLiteral = new DynamicEnumerationLiteral(id, enumeration) { Key = key, Name = name };
        enumeration.AddLiterals([enumerationLiteral]);
        return enumerationLiteral;
    }

    /// <inheritdoc cref="Field"/>
    public static DynamicField Field(this DynamicStructuredDataType structuredDataType, NodeId id,
        MetaPointerKey key, string name)
    {
        var field = new DynamicField(id, structuredDataType) { Key = key, Name = name };
        structuredDataType.AddFields([field]);
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
        var @interface = new DynamicInterface(id, language) { Key = key, Name = name };
        language.AddEntities([@interface]);
        return @interface;
    }

    /// <inheritdoc cref="PrimitiveType"/>
    public static DynamicPrimitiveType PrimitiveType(this DynamicLanguage language, NodeId id, MetaPointerKey key, string name)
    {
        var primitiveType = new DynamicPrimitiveType(id, language) { Key = key, Name = name };
        language.AddEntities([primitiveType]);
        return primitiveType;
    }

    /// <inheritdoc cref="Property"/>
    public static DynamicProperty Property(this DynamicClassifier classifier, NodeId id, MetaPointerKey key, string name)
    {
        var property = new DynamicProperty(id, classifier) { Key = key, Name = name };
        classifier.AddFeatures([property]);
        return property;
    }

    /// <inheritdoc cref="Reference"/>
    public static DynamicReference Reference(this DynamicClassifier classifier, NodeId id, MetaPointerKey key, string name)
    {
        var reference = new DynamicReference(id, classifier) { Key = key, Name = name };
        classifier.AddFeatures([reference]);
        return reference;
    }

    /// <inheritdoc cref="StructuredDataType"/>
    public static DynamicStructuredDataType StructuredDataType(this DynamicLanguage language, NodeId id, MetaPointerKey key,
        string name)
    {
        var structuredDataType = new DynamicStructuredDataType(id, language) { Key = key, Name = name };
        language.AddEntities([structuredDataType]);
        return structuredDataType;
    }
}