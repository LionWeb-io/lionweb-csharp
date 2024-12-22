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

namespace LionWeb.Core.M2;

using M3;
using Serialization;

public partial class LanguageDeserializer
{
    /// <inheritdoc />
    public override void Process(SerializedNode serializedNode)
    {
        _serializedNodesById[Compress(serializedNode.Id)] = serializedNode;
        if (!IsLanguageNode(serializedNode))
            return;

        ProcessInternal(serializedNode, id => CreateNodeWithProperties(serializedNode, id));
    }

    private DynamicIKeyed CreateNodeWithProperties(SerializedNode serializedNode, string id)
    {
        var serializedPropertiesByKey = serializedNode.Properties.ToDictionary(
            serializedProperty => serializedProperty.Property.Key,
            serializedProperty => serializedProperty.Value
        );
        string key = LookupString(_m3.IKeyed_key);
        string name = LookupString(_builtIns.INamed_name);

        return serializedNode.Classifier switch
        {
            var s when s.Key == _m3.Annotation.Key => new DynamicAnnotation(id, null) { Key = key, Name = name },
            var s when s.Key == _m3.Concept.Key => new DynamicConcept(id, null)
            {
                Key = key,
                Name = name,
                Abstract = LookupBool(_m3.Concept_abstract),
                Partition = LookupBool(_m3.Concept_partition)
            },
            var s when s.Key == _m3.Containment.Key => new DynamicContainment(id, null)
            {
                Key = key,
                Name = name,
                Optional = LookupBool(_m3.Feature_optional),
                Multiple = LookupBool(_m3.Link_multiple)
            },
            var s when s.Key == _m3.Enumeration.Key => new DynamicEnumeration(id, null) { Key = key, Name = name },
            var s when s.Key == _m3.EnumerationLiteral.Key => new DynamicEnumerationLiteral(id, null)
            {
                Key = key, Name = name
            },
            var s when s.Key == _m3.Interface.Key => new DynamicInterface(id, null) { Key = key, Name = name },
            var s when s.Key == _m3.Language.Key => new DynamicLanguage(id, LionWebVersion)
            {
                Key = key, Name = name, Version = LookupString(_m3.Language_version)
            },
            var s when s.Key == _m3.PrimitiveType.Key => new DynamicPrimitiveType(id, null) { Key = key, Name = name },
            var s when s.Key == _m3.Property.Key => new DynamicProperty(id, null)
            {
                Key = key, Name = name, Optional = LookupBool(_m3.Feature_optional)
            },
            var s when s.Key == _m3.Reference.Key => new DynamicReference(id, null)
            {
                Key = key,
                Name = name,
                Optional = LookupBool(_m3.Feature_optional),
                Multiple = LookupBool(_m3.Link_multiple)
            },
            _ => throw new UnsupportedClassifierException(serializedNode.Classifier)
        };

        bool LookupBool(Property property)
        {
            if (serializedPropertiesByKey.TryGetValue(property.Key, out var value))
                return value == "true";

            var result = _handler.InvalidPropertyValue<bool>(null, property, Compress(id));
            return result as bool? ?? throw new InvalidValueException(property, result);
        }

        string LookupString(Property property)
        {
            if (serializedPropertiesByKey.TryGetValue(property.Key, out var s) && s != null)
                return s;

            var result = _handler.InvalidPropertyValue<string>(null, property, Compress(id));
            return result as string ?? throw new InvalidValueException(property, result);
        }
    }
}