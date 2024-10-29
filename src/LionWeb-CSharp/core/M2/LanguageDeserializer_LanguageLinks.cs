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

using M1;
using M3;
using Serialization;
using Utilities;

public partial class LanguageDeserializer
{
    private void InstallLanguageLinks()
    {
        foreach (var serializedNode in _serializedNodesById.Values.Where(IsLanguageNode))
        {
            InstallContainments(serializedNode);
            InstallReferences(serializedNode);
        }
    }

    private void InstallContainments(SerializedNode serializedNode)
    {
        var node = _deserializedNodesById[Compress(serializedNode.Id)];

        ILookup<CompressedMetaPointer, IKeyed> serializedContainmentsLookup = serializedNode
            .Containments
            .SelectMany(containment => containment.Children.Select(child => (containment, child)))
            .ToLookup(pair => Compress(pair.containment.Containment), pair => LookupNode<IKeyed>(pair.child));

        if (serializedContainmentsLookup.Count == 0)
            return;

        switch (node)
        {
            case DynamicClassifier classifier:
                classifier.AddFeatures(Lookup<Feature>(_m3.Classifier_features));
                return;
            case DynamicEnumeration enumeration:
                enumeration.AddLiterals(Lookup<EnumerationLiteral>(_m3.Enumeration_literals));
                return;
            case DynamicLanguage language:
                language.AddEntities(Lookup<LanguageEntity>(_m3.Language_entities));
                return;
            case DynamicEnumerationLiteral or DynamicPrimitiveType or DynamicFeature:
                return;
            case IWritableNode writable:
                foreach (var grouping in serializedContainmentsLookup)
                {
                    var feature = _deserializerMetaInfo.FindFeature<Containment>(writable, grouping.Key);
                    if (feature == null)
                        continue;

                    writable.Set(feature, ToIEnumerable(grouping.GetEnumerator()));
                }

                return;

            default:
                Handler.InvalidContainment(node);
                return;
        }

        IEnumerable<T> Lookup<T>(Containment containment) where T : class
        {
            var compressedMetaPointer = Compress(containment.ToMetaPointer());
            if (serializedContainmentsLookup.Contains(compressedMetaPointer))
                return serializedContainmentsLookup[compressedMetaPointer].Cast<T>();

            var serializedContainment =
                serializedNode.Containments.FirstOrDefault(c => c.Containment.Matches(containment));
            if (serializedContainment == null)
                return [];

            return serializedContainment.Children.Select(c =>
            {
                var targetNode = Handler.UnresolvableChild(Compress(c), containment, (IWritableNode)node);
                if (targetNode is T result)
                    return result;
                return null;
            }).Where(t => t != null)!;
        }
    }

    private void InstallReferences(SerializedNode serializedNode)
    {
        var node = _deserializedNodesById[Compress(serializedNode.Id)];
        ILookup<CompressedMetaPointer, IKeyed?> serializedReferencesLookup = serializedNode
            .References
            .SelectMany(reference => reference.Targets.Select(target => (reference, target)))
            .ToLookup(pair => Compress(pair.reference.Reference),
                pair => pair.target.Reference != null ? LookupNode<IKeyed>(pair.target.Reference) : null);

        if (serializedReferencesLookup.Count == 0)
            return;

        switch (node)
        {
            case DynamicLanguage language:
                language.AddDependsOn(LookupMulti<Language>(_m3.Language_dependsOn));
                return;
            case DynamicAnnotation annotation:
                annotation.Extends = LookupSingle<Annotation>(_m3.Annotation_extends);
                annotation.AddImplements(LookupFilteredInterfaces(_m3.Annotation_implements, annotation.Implements));
                annotation.Annotates = LookupSingle<Classifier>(_m3.Annotation_annotates);
                return;
            case DynamicConcept concept:
                concept.Extends = LookupSingle<Concept>(_m3.Concept_extends);
                concept.AddImplements(LookupFilteredInterfaces(_m3.Concept_implements, concept.Implements));
                return;
            case DynamicInterface @interface:
                @interface.AddExtends(LookupFilteredInterfaces(_m3.Interface_extends, @interface.Extends));
                return;
            case DynamicLink link:
                link.Type = LookupSingle<Classifier>(_m3.Link_type);
                return;
            case DynamicProperty property:
                property.Type = LookupSingle<Datatype>(_m3.Property_type);
                return;
            case DynamicEnumeration or DynamicEnumerationLiteral or DynamicPrimitiveType:
                return;
            case IWritableNode writable:
                foreach (var grouping in serializedReferencesLookup)
                {
                    var feature = _deserializerMetaInfo.FindFeature<Reference>(writable, grouping.Key);
                    if (feature == null)
                        continue;

                    writable.Set(feature, ToIEnumerable(grouping.GetEnumerator()));
                }

                return;

            default:
                Handler.InvalidReference(node);
                return;
        }

        T? LookupSingle<T>(Reference reference) where T : class
        {
            var compressedMetaPointer = Compress(reference.ToMetaPointer());
            if (serializedReferencesLookup.Contains(compressedMetaPointer))
            {
                var elements = serializedReferencesLookup[compressedMetaPointer].ToList();
                if (elements.Count == 1)
                    return elements.Cast<T>().First();
            }

            var serializedReference = FindSerializedReference(reference);
            if (serializedReference == null)
                return null;

            if (serializedReference.Targets.Length == 1)
            {
                var target = serializedReference.Targets.First();
                return UnknownReference<T>(reference, target);
            }

            return null;
        }

        IEnumerable<T> LookupMulti<T>(Reference reference) where T : class
        {
            var compressedMetaPointer = Compress(reference.ToMetaPointer());
            if (serializedReferencesLookup.Contains(compressedMetaPointer))
                return serializedReferencesLookup[compressedMetaPointer].Cast<T>();

            var serializedReference = FindSerializedReference(reference);
            if (serializedReference == null)
                return [];

            return serializedReference.Targets.Select(t => UnknownReference<T>(reference, t)).Where(t => t != null)!;
        }

        IEnumerable<Interface> LookupFilteredInterfaces(Reference reference, IEnumerable<Interface> linkedInterfaces) =>
            LookupMulti<Interface>(reference).Except(linkedInterfaces, new LanguageEntityIdentityComparer())
                .OfType<Interface>();

        SerializedReference? FindSerializedReference(Reference reference) =>
            serializedNode.References.FirstOrDefault(r => r.Reference.Matches(reference));

        T? UnknownReference<T>(Feature reference, SerializedReferenceTarget target) where T : class
        {
            var targetNode =
                Handler.UnresolvableReferenceTarget(target.Reference != null ? Compress(target.Reference) : null,
                    target.ResolveInfo, reference, (IWritableNode)node);
            if (targetNode is T result)
                return result;
            return null;
        }
    }

    private IEnumerable<T> ToIEnumerable<T>(IEnumerator<T> enumerator)
    {
        using (enumerator)
        {
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (result == null)
                    continue;
                yield return result;
            }
        }
    }
}