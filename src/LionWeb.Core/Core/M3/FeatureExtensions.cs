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
/// Extensions methods that add methods to M3 types to manipulate their features' values.
/// </summary>
public static class FeatureExtensions
{
    /// <inheritdoc cref="Annotation.Annotates"/>
    public static DynamicAnnotation Annotating(this DynamicAnnotation annotation, Classifier classifier)
    {
        annotation.Annotates = classifier;
        return annotation;
    }

    /// <inheritdoc cref="Annotation.Extends"/>
    public static DynamicAnnotation Extending(this DynamicAnnotation annotation, Annotation extends)
    {
        annotation.Extends = extends;
        return annotation;
    }

    /// <inheritdoc cref="Annotation.Implements"/>
    public static DynamicAnnotation Implementing(this DynamicAnnotation annotation, params Interface[] interfaces)
    {
        annotation.AddImplements(interfaces);
        return annotation;
    }


    /// <inheritdoc cref="Concept.Abstract"/>
    public static DynamicConcept IsAbstract(this DynamicConcept concept, bool value = true)
    {
        concept.Abstract = value;
        return concept;
    }

    /// <inheritdoc cref="Concept.Partition"/>
    public static DynamicConcept IsPartition(this DynamicConcept concept, bool value = true)
    {
        concept.Partition = value;
        return concept;
    }

    /// <inheritdoc cref="Concept.Extends"/>
    public static DynamicConcept Extending(this DynamicConcept concept, Concept extends)
    {
        concept.Extends = extends;
        return concept;
    }

    /// <inheritdoc cref="Concept.Implements"/>
    public static DynamicConcept Implementing(this DynamicConcept concept, params Interface[] interfaces)
    {
        concept.AddImplements(interfaces);
        return concept;
    }

    /// <inheritdoc cref="Feature.Optional"/>
    public static T IsOptional<T>(this T feature, bool value = true) where T : DynamicFeature
    {
        feature.Optional = value;
        return feature;
    }

    /// <inheritdoc cref="Property.Type"/>
    public static DynamicProperty OfType(this DynamicProperty property, Datatype type)
    {
        property.Type = type;
        return property;
    }

    /// <inheritdoc cref="Link.Multiple"/>
    public static T IsMultiple<T>(this T link, bool value = true) where T : DynamicLink
    {
        link.Multiple = value;
        return link;
    }

    /// <inheritdoc cref="Link.Type"/>
    public static T OfType<T>(this T link, Classifier type) where T : DynamicLink
    {
        link.Type = type;
        return link;
    }


    /// <inheritdoc cref="Interface.Extends"/>
    public static DynamicInterface Extending(this DynamicInterface @interface, params Interface[] interfaces)
    {
        @interface.AddExtends(interfaces);
        return @interface;
    }
}