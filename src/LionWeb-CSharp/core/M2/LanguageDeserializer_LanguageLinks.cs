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
        if (!_deserializedNodesById.TryGetValue(Compress(serializedNode.Id), out var node))
            return;

        ILookup<string, IKeyed> serializedContainmentsLookup = serializedNode
            .Containments
            .SelectMany(containment => containment
                .Children
                .Select(child => (containment.Containment, child))
            )
            .ToLookup(
                pair => pair.Containment.Key,
                pair =>
                {
                    var compressedId = Compress(pair.child);
                    return (IKeyed)(_deserializedNodesById.TryGetValue(compressedId, out var deserialized)
                        ? deserialized
                        : _dependentNodesById[compressedId]);
                }
            );

        if (serializedContainmentsLookup.Count == 0)
            return;

        _languageVersionSpecifics.InstallLanguageContainments(serializedNode, node, serializedContainmentsLookup);
    }

    private void InstallLanguageReferences(SerializedNode serializedNode)
    {
        if (!_deserializedNodesById.TryGetValue(Compress(serializedNode.Id), out var node))
            return;

        ILookup<string, IKeyed?> serializedReferencesLookup = serializedNode
            .References
            .SelectMany(reference => reference.Targets.Select(target => (reference, target)))
            .ToLookup(pair => pair.reference.Reference.Key,
                pair => FindReferenceTarget(CompressOpt(pair.target.Reference), pair.target.ResolveInfo) as IKeyed);

        if (serializedReferencesLookup.Count == 0)
            return;

_languageVersionSpecifics.InstallLanguageReferences(serializedNode, node, serializedReferencesLookup);
    }
}