// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Migration;

using M1;
using M2;
using M3;
using Serialization;

/// Integrates unknown language elements during deserialization as good as possible.
/// <seealso cref="ModelMigrator"/>
public class MigrationDeserializerHandler : DeserializerDelegatingHandler
{
    private readonly LionWebVersions _lionWebVersion;
    private readonly CompressedIdConfig _compressedIdConfig;
    private readonly List<DynamicLanguage> _languages;
    private readonly Dictionary<ICompressedId, List<DynamicLanguage>> _languageKeys = [];

    /// <inheritdoc />
    public MigrationDeserializerHandler(LionWebVersions lionWebVersion, IEnumerable<DynamicLanguage> languages,
        IDeserializerHandler delegateHandler) : base(delegateHandler)
    {
        _lionWebVersion = lionWebVersion;
        _compressedIdConfig = new CompressedIdConfig(false);
        _languages = languages.ToList();
        foreach (var language in _languages)
        {
            AddLanguage(language);
        }
    }

    private void AddLanguage(DynamicLanguage language)
    {
        if (_languageKeys.TryGetValue(Compress(language.Key), out var langs))
        {
            langs.Add(language);
        } else
        {
            _languageKeys[Compress(language.Key)] = [language];
        }
    }

    /// <inheritdoc />
    public override Classifier? UnknownClassifier(CompressedMetaPointer classifier, ICompressedId id) =>
        CreateClassifier(classifier);

    private DynamicConcept CreateClassifier(CompressedMetaPointer classifier)
    {
        DynamicLanguage lang = GetOrCreateLanguage(classifier);

        var result = new DynamicConcept(classifier.Key.Original!, _lionWebVersion, null)
        {
            Name = "Concept-" + classifier.Key.Original!, Key = classifier.Key.Original!
        };
        lang.AddEntities([result]);
        return result;
    }

    private DynamicLanguage GetOrCreateLanguage(CompressedMetaPointer metaPointer)
    {
        DynamicLanguage? lang = null;
        if (_languageKeys.TryGetValue(metaPointer.Language, out var languages))
        {
            lang = languages.FirstOrDefault(l => Equals(Compress(l.Version), metaPointer.Version));
        }

        lang ??= CreateLanguage(metaPointer);
        return lang;
    }

    private DynamicLanguage CreateLanguage(CompressedMetaPointer classifier)
    {
        var language = classifier.Language.Original!;
        var version = classifier.Version.Original!;

        var lang = new DynamicLanguage(language, _lionWebVersion)
        {
            Name = "Language-" + language, Key = language, Version = version
        };
        lang.SetFactory(new MigrationFactory(lang));
        _languages.Add(lang);
        AddLanguage(lang);
        return lang;
    }

    /// <inheritdoc />
    public override Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier,
        IReadableNode node)
    {
        DynamicFeature? result = null;
        if (typeof(TFeature).IsAssignableFrom(typeof(Property)))
        {
            result = new DynamicProperty(feature.Key.Original!, _lionWebVersion, null)
            {
                Name = feature.Key.Original!,
                Key = feature.Key.Original!,
                Optional = true,
                Type = _lionWebVersion.BuiltIns.String
            };
        } else if (typeof(TFeature).IsAssignableFrom(typeof(Containment)))
        {
            result = new DynamicContainment(feature.Key.Original!, _lionWebVersion, null)
            {
                Name = feature.Key.Original!,
                Key = feature.Key.Original!,
                Multiple = true,
                Optional = true,
                Type = _lionWebVersion.BuiltIns.Node
            };
        } else if (typeof(TFeature).IsAssignableFrom(typeof(Reference)))
        {
            result = new DynamicReference(feature.Key.Original!, _lionWebVersion, null)
            {
                Name = feature.Key.Original!,
                Key = feature.Key.Original!,
                Multiple = true,
                Optional = true,
                Type = _lionWebVersion.BuiltIns.Node
            };
        } else
        {
            throw new ArgumentOutOfRangeException(feature.ToString());
        }

        var language = GetOrCreateLanguage(feature);
        var defaultClassifierKey = $"{feature.Language.Original}_defaultClassifier";
        var defaultClassifier =
            language.Entities.OfType<DynamicConcept>().FirstOrDefault(e => e.Key == defaultClassifierKey) ??
            CreateClassifier(CompressedMetaPointer.Create(
                new MetaPointer(feature.Language.Original!, feature.Version.Original!, defaultClassifierKey),
                _compressedIdConfig));

        defaultClassifier.AddFeatures([result]);
        return result;
    }

    /// <inheritdoc />
    public override IWritableNode? InvalidAnnotation(IReadableNode annotation, IReadableNode? node)
    {
        var oldClassifier = (DynamicClassifier)annotation.GetClassifier();
        var language = (DynamicLanguage)oldClassifier.GetLanguage();

        var newClassifier = new DynamicAnnotation(oldClassifier.GetId(), language.LionWebVersion, language)
        {
            Key = oldClassifier.Key, Name = oldClassifier.Name, Annotates = _lionWebVersion.BuiltIns.Node
        };
        oldClassifier.ReplaceWith(newClassifier);

        newClassifier.AddAnnotations(oldClassifier.GetAnnotations());
        newClassifier.AddFeatures(oldClassifier.Features);

        var lenientAnnotationInstance = ((LenientNode)annotation);
        lenientAnnotationInstance.SetClassifier(newClassifier);
        return lenientAnnotationInstance;
    }

    private ICompressedId Compress(string key) =>
        ICompressedId.Create(key, _compressedIdConfig);
}