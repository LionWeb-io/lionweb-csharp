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

namespace LionWeb.Core.M3;

// The types here implement the LionCore M3.

/// <inheritdoc cref="EnumerationLiteral"/>
public class DynamicEnumerationLiteral : DynamicIKeyed, EnumerationLiteral
{
    /// <inheritdoc />
    public DynamicEnumerationLiteral(NodeId id, LionWebVersions lionWebVersion, DynamicEnumeration? enumeration)
        : base(id, lionWebVersion)
    {
        enumeration?.AddLiteralsRaw(this);
        _parent = enumeration;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.EnumerationLiteral;
}