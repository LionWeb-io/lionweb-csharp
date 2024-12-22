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

/// <inheritdoc cref="ILanguageDeserializer"/>
public partial class LanguageDeserializer : DeserializerBase<IReadableNode>, ILanguageDeserializer
{
    private readonly Dictionary<CompressedId, SerializedNode> _serializedNodesById = new();

    private readonly DeserializerBuilder _deserializerBuilder = new();

    /// <summary>
    /// Deserializes languages based on LionWeb version <paramref name="lionWebVersion"/>.
    /// </summary>
    /// <inheritdoc />
    public LanguageDeserializer(LionWebVersions lionWebVersion, IDeserializerHandler? handler = null) :
        base(lionWebVersion, handler)
    {
        RegisterDependentLanguage(_m3);
    }

    /// <inheritdoc />
    public override void RegisterInstantiatedLanguage(Language language, INodeFactory? factory = null)
    {
        base.RegisterInstantiatedLanguage(language, factory);
        _deserializerBuilder.WithCustomFactory(language, factory ?? language.GetFactory());
    }

    /// <inheritdoc />
    public void RegisterDependentLanguage(Language language)
    {
        _deserializerBuilder.WithLanguage(language);
        RegisterDependentNodes(M1Extensions.Descendants<IKeyed>(language, true, true));
    }

    /// <inheritdoc />
    public override void RegisterDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        var nodes = dependentNodes.ToList();
        base.RegisterDependentNodes(nodes);
        _deserializerBuilder.WithDependentNodes(nodes);
    }

    private bool IsLanguageNode(SerializedNode serializedNode) =>
        serializedNode.Classifier.Language == _m3.Key;
}