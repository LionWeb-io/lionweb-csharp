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
using Utilities;
using VersionSpecific.V2023_1;
using VersionSpecific.V2024_1_Compatible;
using VersionSpecific.V2024_1;
using VersionSpecific.V2025_1_Compatible;
using VersionSpecific.V2025_1;

internal interface ILanguageDeserializerVersionSpecifics : IVersionSpecifics
{
    /// <summary>
    /// Creates an instance of <see cref="ILanguageDeserializerVersionSpecifics"/> that implements <paramref name="lionWebVersion"/>.
    /// </summary>
    /// <exception cref="UnsupportedVersionException"></exception>
    public static ILanguageDeserializerVersionSpecifics Create(LionWebVersions lionWebVersion,
        LanguageDeserializer deserializer, ILanguageDeserializerHandler handler)
        => lionWebVersion switch
        {
            IVersion2023_1 => new LanguageDeserializerVersionSpecifics_2023_1(deserializer, handler),
            IVersion2024_1 => new LanguageDeserializerVersionSpecifics_2024_1(deserializer, handler),
            IVersion2024_1_Compatible =>
                new LanguageDeserializerVersionSpecifics_2024_1_Compatible(deserializer, handler),
            IVersion2025_1 => new LanguageDeserializerVersionSpecifics_2025_1(deserializer, handler),
            IVersion2025_1_Compatible =>
                new LanguageDeserializerVersionSpecifics_2025_1_Compatible(deserializer, handler),
            _ => throw new UnsupportedVersionException(lionWebVersion)
        };

    /// <seealso cref="LanguageDeserializer.CreateNodeWithProperties"/>
    public DynamicIKeyed CreateNodeWithProperties(SerializedNode serializedNode, NodeId id);

    /// <seealso cref="LanguageDeserializer.InstallLanguageContainments"/>
    public void InstallLanguageContainments(SerializedNode serializedNode, IReadableNode node,
        ILookup<MetaPointerKey, IKeyed> serializedContainmentsLookup);

    /// <seealso cref="LanguageDeserializer.InstallLanguageReferences"/>
    public void InstallLanguageReferences(SerializedNode serializedNode, IReadableNode node,
        ILookup<MetaPointerKey, IKeyed?> serializedReferencesLookup);
}

internal abstract class LanguageDeserializerVersionSpecificsBase(
    LanguageDeserializer deserializer,
    ILanguageDeserializerHandler handler)
    : ILanguageDeserializerVersionSpecifics
{
    internal readonly LanguageDeserializer _deserializer = deserializer;
    internal readonly ILanguageDeserializerHandler _handler = handler;

    public abstract LionWebVersions Version { get; }

    public abstract DynamicIKeyed CreateNodeWithProperties(SerializedNode serializedNode, NodeId id);

    public abstract void InstallLanguageContainments(SerializedNode serializedNode, IReadableNode node,
        ILookup<MetaPointerKey, IKeyed> serializedContainmentsLookup);

    public abstract void InstallLanguageReferences(SerializedNode serializedNode, IReadableNode node,
        ILookup<MetaPointerKey, IKeyed?> serializedReferencesLookup);
}

internal abstract class NodeCreatorBase
{
    private readonly LanguageDeserializerVersionSpecificsBase _versionSpecifics;
    internal readonly SerializedNode _serializedNode;
    private readonly Dictionary<MetaPointerKey, PropertyValue?> _serializedPropertiesByKey;
    internal readonly MetaPointerKey _key;
    internal readonly string _name;
    internal readonly NodeId _id;

    internal NodeCreatorBase(LanguageDeserializerVersionSpecificsBase versionSpecifics, SerializedNode serializedNode,
        NodeId id)
    {
        _versionSpecifics = versionSpecifics;
        _serializedNode = serializedNode;
        _id = id;
        _serializedPropertiesByKey = serializedNode.Properties.ToDictionary(
            serializedProperty => serializedProperty.Property.Key,
            serializedProperty => serializedProperty.Value
        );
        _key = LookupString(LionCore.IKeyed_key);
        _name = LookupString(BuiltIns.INamed_name);
    }

    public virtual DynamicIKeyed Create() => _serializedNode.Classifier switch
    {
        var s when s.Key == LionCore.Annotation.Key => new DynamicAnnotation(_id, _versionSpecifics.Version, null) { Key = _key, Name = _name },
        var s when s.Key == LionCore.Concept.Key => new DynamicConcept(_id, _versionSpecifics.Version, null)
        {
            Key = _key,
            Name = _name,
            Abstract = LookupBool(LionCore.Concept_abstract),
            Partition = LookupBool(LionCore.Concept_partition)
        },
        var s when s.Key == LionCore.Containment.Key => new DynamicContainment(_id, _versionSpecifics.Version, null)
        {
            Key = _key,
            Name = _name,
            Optional = LookupBool(LionCore.Feature_optional),
            Multiple = LookupBool(LionCore.Link_multiple)
        },
        var s when s.Key == LionCore.Enumeration.Key => new DynamicEnumeration(_id, _versionSpecifics.Version, null) { Key = _key, Name = _name },
        var s when s.Key == LionCore.EnumerationLiteral.Key => new DynamicEnumerationLiteral(_id, _versionSpecifics.Version, null)
        {
            Key = _key, Name = _name
        },
        var s when s.Key == LionCore.Interface.Key => new DynamicInterface(_id, _versionSpecifics.Version, null) { Key = _key, Name = _name },
        var s when s.Key == LionCore.Language.Key => new DynamicLanguage(_id, Version)
        {
            Key = _key, Name = _name, Version = LookupString(LionCore.Language_version)
        },
        var s when s.Key == LionCore.PrimitiveType.Key => new DynamicPrimitiveType(_id, _versionSpecifics.Version, null)
        {
            Key = _key, Name = _name
        },
        var s when s.Key == LionCore.Property.Key => new DynamicProperty(_id, _versionSpecifics.Version, null)
        {
            Key = _key, Name = _name, Optional = LookupBool(LionCore.Feature_optional)
        },
        var s when s.Key == LionCore.Reference.Key => new DynamicReference(_id, _versionSpecifics.Version, null)
        {
            Key = _key,
            Name = _name,
            Optional = LookupBool(LionCore.Feature_optional),
            Multiple = LookupBool(LionCore.Link_multiple)
        },
        _ => throw new UnsupportedClassifierException(_serializedNode.Classifier)
    };

    protected abstract LionWebVersions Version { get; }
    protected virtual ILionCoreLanguage LionCore => Version.LionCore;
    private IBuiltInsLanguage BuiltIns => Version.BuiltIns;

    private bool LookupBool(Property property)
    {
        if (_serializedPropertiesByKey.TryGetValue(property.Key, out var value))
            return value == "true";

        var result =
            _versionSpecifics._handler.InvalidPropertyValue<bool>(null, property,
                _versionSpecifics._deserializer.Compress(_id));
        return result as bool? ?? throw new InvalidValueException(property, result);
    }

    private string LookupString(Property property)
    {
        if (_serializedPropertiesByKey.TryGetValue(property.Key, out var s) && s != null)
            return s;

        var result =
            _versionSpecifics._handler.InvalidPropertyValue<string>(null, property,
                _versionSpecifics._deserializer.Compress(_id));
        return result as string ?? throw new InvalidValueException(property, result);
    }
}

internal abstract class ContainmentsInstallerBase(
    LanguageDeserializerVersionSpecificsBase versionSpecifics,
    SerializedNode serializedNode,
    IReadableNode node,
    ILookup<MetaPointerKey, IKeyed> serializedContainmentsLookup)
{
    internal readonly LanguageDeserializerVersionSpecificsBase _versionSpecifics = versionSpecifics;
    internal readonly IReadableNode _node = node;

    public virtual void Install()
    {
        switch (_node)
        {
            case DynamicClassifier classifier:
                classifier.AddFeatures(Lookup<Feature>(LionCore.Classifier_features));
                return;
            case DynamicEnumeration enumeration:
                enumeration.AddLiterals(Lookup<EnumerationLiteral>(LionCore.Enumeration_literals));
                return;
            case DynamicLanguage language:
                language.AddEntities(Lookup<LanguageEntity>(LionCore.Language_entities));
                return;
            case DynamicEnumerationLiteral or DynamicPrimitiveType or DynamicFeature:
                return;
            default:
                _versionSpecifics._handler.InvalidContainment(_node);
                return;
        }
    }

    protected abstract LionWebVersions Version { get; }
    protected virtual ILionCoreLanguage LionCore => Version.LionCore;

    internal IEnumerable<T> Lookup<T>(Containment containment) where T : class
    {
        if (serializedContainmentsLookup.Contains(containment.Key))
            return serializedContainmentsLookup[containment.Key].Cast<T>();

        var serializedContainment =
            serializedNode.Containments.FirstOrDefault(c => c.Containment.Matches(containment));
        if (serializedContainment == null)
            return [];

        return serializedContainment.Children
            .Select(c =>
                _versionSpecifics._handler.UnresolvableChild(_versionSpecifics._deserializer.Compress(c), containment,
                    _node) as T)
            .Where(t => t != null)!;
    }
}

internal abstract class ReferencesInstallerBase(
    LanguageDeserializerVersionSpecificsBase versionSpecifics,
    SerializedNode serializedNode,
    IReadableNode node,
    ILookup<MetaPointerKey, IKeyed?> serializedReferencesLookup)
{
    internal readonly IReadableNode _node = node;
    internal readonly LanguageDeserializerVersionSpecificsBase _versionSpecifics = versionSpecifics;

    public virtual void Install()
    {
        switch (_node)
        {
            case DynamicLanguage language:
                language.AddDependsOn(LookupMulti<Language>(LionCore.Language_dependsOn));
                return;
            case DynamicAnnotation annotation:
                annotation.Extends = LookupSingle<Annotation>(LionCore.Annotation_extends);
                annotation.AddImplements(
                    LookupFilteredInterfaces(LionCore.Annotation_implements, annotation.Implements));
                annotation.Annotates = LookupSingle<Classifier>(LionCore.Annotation_annotates)!;
                return;
            case DynamicConcept concept:
                concept.Extends = LookupSingle<Concept>(LionCore.Concept_extends);
                concept.AddImplements(LookupFilteredInterfaces(LionCore.Concept_implements, concept.Implements));
                return;
            case DynamicInterface @interface:
                @interface.AddExtends(LookupFilteredInterfaces(LionCore.Interface_extends, @interface.Extends));
                return;
            case DynamicLink link:
                link.Type = LookupSingle<Classifier>(LionCore.Link_type)!;
                return;
            case DynamicProperty property:
                property.Type = LookupSingle<Datatype>(LionCore.Property_type)!;
                return;
            case DynamicEnumeration or DynamicEnumerationLiteral or DynamicPrimitiveType:
                return;

            default:
                _versionSpecifics._handler.InvalidReference(_node);
                return;
        }
    }

    protected abstract LionWebVersions Version { get; }
    protected virtual ILionCoreLanguage LionCore => Version.LionCore;

    internal T? LookupSingle<T>(Reference reference) where T : class
    {
        if (serializedReferencesLookup.Contains(reference.Key))
        {
            var elements = serializedReferencesLookup[reference.Key].ToList();
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

    private IEnumerable<T> LookupMulti<T>(Reference reference) where T : class
    {
        if (serializedReferencesLookup.Contains(reference.Key))
            return serializedReferencesLookup[reference.Key].Cast<T>();

        var serializedReference = FindSerializedReference(reference);
        if (serializedReference == null)
            return [];

        return serializedReference.Targets.Select(t => UnknownReference<T>(reference, t)).Where(t => t != null)!;
    }

    private IEnumerable<Interface> LookupFilteredInterfaces(Reference reference,
        IEnumerable<Interface> linkedInterfaces) =>
        LookupMulti<Interface>(reference)
            .Except(linkedInterfaces, new LanguageEntityIdentityComparer())
            .OfType<Interface>();

    private SerializedReference? FindSerializedReference(Reference reference) =>
        serializedNode.References
            .FirstOrDefault(r => r.Reference.Matches(reference));

    private T? UnknownReference<T>(Feature reference, SerializedReferenceTarget target) where T : class =>
        _versionSpecifics._handler.UnresolvableReferenceTarget(
            _versionSpecifics._deserializer.CompressOpt(target.Reference), target.ResolveInfo, reference,
            (IWritableNode)_node) as T;
}