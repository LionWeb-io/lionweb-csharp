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
    private void InstallLanguageLinks()
    {
        foreach (var serializedNode in _serializedNodesById.Values.Where(IsLanguageNode))
        {
            InstallLanguageContainments(serializedNode);
            InstallLanguageReferences(serializedNode);
        }

        foreach (var language in _deserializedNodesById.Values.OfType<Language>())
        {
            _deserializerMetaInfo.RegisterInstantiatedLanguage(language, language.GetFactory());
        }
    }

    private void InstallLanguageContainments(SerializedNode serializedNode)
    {
        if (!_deserializedNodesById.TryGetValue(serializedNode.Id, out var node))
            return;

        List<(MetaPointerKey, IKeyed)> containments = [];
        foreach (var serializedContainment in serializedNode.Containments)
        {
            foreach (var childId in serializedContainment.Children)
            {
                var keyed = (IKeyed)
                    (_deserializedNodesById.TryGetValue(childId, out var deserialized)
                        ? deserialized
                        : _dependentNodesById[childId]);
                containments.Add((serializedContainment.Containment.Key, keyed));
            }
        }

        if (containments.Count == 0)
            return;

        ILookup<MetaPointerKey, IKeyed> serializedContainmentsLookup = containments.ToLookup(p => p.Item1, p => p.Item2);
        _languageVersionSpecifics.InstallLanguageContainments(serializedNode, node, serializedContainmentsLookup);
    }

    private void InstallLanguageReferences(SerializedNode serializedNode)
    {
        if (!_deserializedNodesById.TryGetValue(serializedNode.Id, out var node))
            return;

        List<(MetaPointerKey, IKeyed?)> references = [];

        foreach (var serializedReference in serializedNode.References)
        {
            foreach (var target in serializedReference.Targets)
            {
                references.Add((serializedReference.Reference.Key, FindReferenceTarget(target.Reference, target.ResolveInfo) as IKeyed));
            }
        }

        if (references.Count == 0)
            return;

        ILookup<MetaPointerKey, IKeyed?> serializedReferencesLookup = references.ToLookup(p => p.Item1, p => p.Item2);
        _languageVersionSpecifics.InstallLanguageReferences(serializedNode, node, serializedReferencesLookup);
    }
}