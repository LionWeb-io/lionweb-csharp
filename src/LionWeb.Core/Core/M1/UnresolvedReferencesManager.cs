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
using System.Diagnostics;

/// <summary>
/// Collects unresolved references, and updates them once a fitting node has been <see cref="Receive">received</see>.
///
/// <para/>
/// Usage:
/// <list type="number">
/// <item><description>
/// Call <see cref="RegisterUnresolvedReference"/> in <see cref="IDeserializerHandler.UnresolvableReferenceTarget"/>
/// </description></item>
/// <item><description>
/// <see cref="INotificationSender.ConnectTo">Connect</see>
/// a <see cref="IForest"/> or <see cref="IPartitionInstance"/>
/// to <c>this</c>.
/// </description></item>
/// </list> 
/// </summary>
public class UnresolvedReferencesManager : INotificationReceiver
{
    private readonly List<(IWritableNode parent, Feature reference, IReferenceTarget target)> _unresolvedReferences = [];
    
    /// Updates unresolved reference to nodes newly added in <paramref name="notification"/>.
    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        if (notification is not INewNodeNotification newNodeNotification)
            return;

        var allDescendants = M1Extensions.Descendants(newNodeNotification.NewNode, true, true).ToDictionary(n => n.GetId());

        _unresolvedReferences.RemoveAll(e => ResolveMatchingReferences(allDescendants, e));
    }

    private bool ResolveMatchingReferences(Dictionary<NodeId, IReadableNode> allDescendants,
        (IWritableNode parent, Feature reference, IReferenceTarget target) e)
    {
        if (e.target.TargetId is null || !allDescendants.TryGetValue(e.target.TargetId, out var newNode))
            return false;

        Log($"Resolving {e.target} in {e.parent.GetId()}.{e.reference}");
        e.target.Target = newNode;

        return true;
    }

    /// Registers unresolved <paramref name="target"/> in <paramref name="reference"/> of <paramref name="parent"/>,
    /// to be resolved later.
    public IReferenceTarget RegisterUnresolvedReference(IWritableNode parent, Feature reference, IReferenceTarget target)
    {
        Log($"Registering {target} in {parent.GetId()}.{reference}");
        _unresolvedReferences.Add((parent, reference, target));
        return target;
    }

    protected virtual void Log(string message) =>
        Debug.WriteLine(message);
}