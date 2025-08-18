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

namespace LionWeb.Core.Notification.Handler;

using Utilities;

public class SameIdCloner : Cloner
{
    public SameIdCloner(IEnumerable<INode> inputNodes) : base(inputNodes)
    {
    }

    /// <inheritdoc cref="Cloner.Clone()"/>
    public static new T Clone<T>(T node) where T : class, INode =>
        (T)new SameIdCloner([node]).Clone()[node];

    protected override NodeId GetNewId(INode remoteNode) =>
        remoteNode.GetId();
}