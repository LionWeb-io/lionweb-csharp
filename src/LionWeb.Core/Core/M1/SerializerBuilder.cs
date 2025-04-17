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

namespace LionWeb.Core.M1;

/// Builds an <see cref="ISerializer"/>.
public class SerializerBuilder
{
    private LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private ISerializerHandler? _handler;
    private CompressedIdConfig _compressedIdConfig = new();
    private bool _serializeEmptyFeatures = true;
    private bool _persistLionCoreReferenceTargetIds = false;

    /// Registers a custom handler.
    /// Defaults to <see cref="SerializerExceptionHandler"/>.
    public SerializerBuilder WithHandler(ISerializerHandler handler)
    {
        _handler = handler;
        return this;
    }

    /// Whether to compress ids, and whether to store uncompressed node and meta-pointer ids
    /// alongside the compressed ones during processing.
    public SerializerBuilder WithCompressedIds(CompressedIdConfig? config = null)
    {
        _compressedIdConfig = config ?? new();
        return this;
    }

    /// The version of LionWeb standard to use.
    /// Defaults to <see cref="LionWebVersions.Current"/>.
    public SerializerBuilder WithLionWebVersion(LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
        return this;
    }

    /// <inheritdoc cref="ISerializer.SerializeEmptyFeatures"/>
    public SerializerBuilder WithSerializedEmptyFeatures(bool serializeEmptyFeatures = true)
    {
        _serializeEmptyFeatures = serializeEmptyFeatures;
        return this;
    }

    /// <inheritdoc cref="Serializer.PersistLionCoreReferenceTargetIds"/>
    public SerializerBuilder WithPersistedLionCoreReferenceTargetIds(bool persistLionCoreReferenceTargetIds = true)
    {
        _persistLionCoreReferenceTargetIds = persistLionCoreReferenceTargetIds;
        return this;
    }

    /// <summary>Builds the serializer.</summary>
    public ISerializer Build()
    {
        Serializer result = new Serializer(_lionWebVersion)
        {
            CompressedIdConfig = _compressedIdConfig,
            Handler = _handler ?? new SerializerExceptionHandler(),
            SerializeEmptyFeatures = _serializeEmptyFeatures,
            PersistLionCoreReferenceTargetIds = _persistLionCoreReferenceTargetIds
        };

        return result;
    }
}