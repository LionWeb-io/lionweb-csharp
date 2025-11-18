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

using M3;
using Notification;
using Notification.Pipe;

/// <summary>
/// Collects unresolved references, and updates them once a fitting node has been <see cref="Receive">received</see>.
/// </summary>
public class UnresolvedReferencesManager : INotificationReceiver
{
    private readonly List<(IWritableNode parent, Feature reference, ReferenceTarget target)> _unresolvedReferences = [];
    
    /// Updates unresolved reference to nodes newly added in <paramref name="notification"/>.
    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        if (notification is not INewNodeNotification newNodeNotification)
            return;

        _unresolvedReferences.RemoveAll(e => ResolveMatchingReferences(newNodeNotification, e));
    }

    private bool ResolveMatchingReferences(INewNodeNotification newNodeNotification, (IWritableNode parent, Feature reference, ReferenceTarget target) e)
    {
        if (e.target.TargetId != newNodeNotification.NewNode.GetId())
            return false;

        e.target.Target = newNodeNotification.NewNode;
        return true;
    }

    /// Registers unresolved <paramref name="target"/> in <paramref name="reference"/> of <paramref name="parent"/>,
    /// to be resolved later.
    public ReferenceTarget RegisterUnresolvedReference(IWritableNode parent, Feature reference, ReferenceTarget target)
    {
        _unresolvedReferences.Add((parent, reference, target));
        return target;
    }
}