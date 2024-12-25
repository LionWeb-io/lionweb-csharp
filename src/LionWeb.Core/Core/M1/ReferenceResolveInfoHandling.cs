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

namespace LionWeb.Core.M1;

using M2;

/// Whether an <see cref="IDeserializer"/> should try to resolve references that only contain a
/// <see cref="LionWeb.Core.Serialization.SerializedReferenceTarget.ResolveInfo"/>.
public enum ReferenceResolveInfoHandling
{
    /// Don't try to resolve by name.
    None,
    
    /// Resolve to node with matching <see cref="INamed.Name"/> if only one matching node is present in deserializer. 
    NameIfUnique,

    /// Resolve to first node with matching <see cref="INamed.Name"/>. 
    Name
}