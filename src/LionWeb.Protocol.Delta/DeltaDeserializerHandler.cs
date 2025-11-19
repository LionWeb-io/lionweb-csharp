// Copyright 2024 TRUMPF Laser GmbH
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
// SPDX-FileCopyrightText: 2024 TRUMPF Laser GmbH
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Protocol.Delta;

using Core;
using Core.M1;
using Core.M3;

/// <summary>
/// This handler enables deserializer to accept node id that appears
/// both in received delta(s) and local nodes. 
/// </summary>
/// <remarks>In the context of delta protocol, this enables replacing a node in a model
/// with a new node with the same id, which results in a valid model.</remarks>
/// <param name="unresolvedReferencesManager">Optional <see cref="UnresolvedReferencesManager"/> to report unresolved references to.</param>
public class DeltaDeserializerHandler(UnresolvedReferencesManager? unresolvedReferencesManager = null) : DeserializerExceptionHandler
{
    /// <inheritdoc/>
    public override bool SkipDeserializingDependentNode(ICompressedId id) =>
        false;

    /// <inheritdoc />
    public override IReferenceTarget? UnresolvableReferenceTarget(IReferenceTarget target,
        Feature reference, IReadableNode parent) =>
        unresolvedReferencesManager?.RegisterUnresolvedReference((IWritableNode)parent, reference, target);
}