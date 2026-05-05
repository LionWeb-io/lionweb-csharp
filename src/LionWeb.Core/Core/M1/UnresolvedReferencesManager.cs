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

using M2;
using M3;
using Notification;
using Notification.Partition;
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
/// <param name="registerAddedUnresolvedReferences">
/// Whether unresolved references inside added nodes should be registered; defaults to <c>false</c>.
/// Enabling this is useful if we want to explicitly create unresolved references in this instance
/// (via <c>node.Set(myReference, new ReferenceTarget("resolveInfo", "targetId", null))</c>).
/// </param>
public class UnresolvedReferencesManager(bool registerAddedUnresolvedReferences = false) : INotificationReceiver
{
    private readonly List<(IWritableNode parent, Feature reference, IReferenceTarget target)> _unresolvedReferences = [];

    /// Updates unresolved reference to nodes newly added in <paramref name="notification"/>.
    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        if (notification is INewNodeNotification newNodeNotification)
        {
            var allDescendants = M1Extensions.Descendants(newNodeNotification.NewNode, true, true).ToDictionary(n => n.GetId());

            if (registerAddedUnresolvedReferences)
                ProcessReferences(allDescendants.Values, RegisterUnresolvedReference);

            _unresolvedReferences.RemoveAll(e => ResolveMatchingReferences(allDescendants, e));
        }

        if (notification is IDeletedNodeNotification deletedNodeNotification)
        {
            ProcessReferences(deletedNodeNotification.DeletedNodes, UnregisterUnresolvedReference);
        }

        switch (notification)
        {
            case ReferenceAddedNotification referenceAddedNotification:
                if (referenceAddedNotification.NewTarget.Target is null)
                    RegisterUnresolvedReference(referenceAddedNotification.Parent, referenceAddedNotification.Reference, referenceAddedNotification.NewTarget);
                return;

            case ReferenceDeletedNotification referenceDeletedNotification:
                UnregisterUnresolvedReference(referenceDeletedNotification.Parent, referenceDeletedNotification.Reference, referenceDeletedNotification.DeletedTarget);
                return;

            case ReferenceChangedNotification referenceChangedNotification:
                UnregisterUnresolvedReference(referenceChangedNotification.Parent, referenceChangedNotification.Reference, referenceChangedNotification.OldTarget);
                if (referenceChangedNotification.NewTarget.Target is null)
                    RegisterUnresolvedReference(referenceChangedNotification.Parent, referenceChangedNotification.Reference, referenceChangedNotification.NewTarget);
                return;
        }
    }

    private static void ProcessReferences(IEnumerable<IReadableNode> nodes, Func<IWritableNode, Reference, IReferenceTarget, IReferenceTarget> action)
    {
        foreach (var node in nodes.OfType<IWritableNode>())
        {
            foreach (var reference in node.GetClassifier().AllFeatures().OfType<Reference>())
            {
                switch (reference.Multiple)
                {
                    case true when node.TryGetReferencesRaw(reference, out var targets):
                        foreach (IReferenceTarget target in targets)
                        {
                            if (target.Target is null)
                                action(node, reference, target);
                        }

                        break;

                    case false when node.TryGetReferenceRaw(reference, out var target) && target is { Target: null }:
                        action(node, reference, target);
                        break;
                }
            }
        }
    }

    private bool ResolveMatchingReferences(Dictionary<NodeId, IReadableNode> allDescendants,
        (IWritableNode parent, Feature reference, IReferenceTarget target) e)
    {
        if (e.target.TargetId is null || !allDescendants.TryGetValue(e.target.TargetId, out var newNode))
            return false;

        LogResolve(e.parent, e.reference, e.target);
        e.target.Target = newNode;

        return true;
    }

    /// Registers unresolved <paramref name="target"/> in <paramref name="reference"/> of <paramref name="parent"/>,
    /// to be resolved later.
    public IReferenceTarget RegisterUnresolvedReference(IWritableNode parent, Feature reference, IReferenceTarget target)
    {
        LogRegister(parent, reference, target);
        _unresolvedReferences.Add((parent, reference, target));
        return target;
    }

    /// Unregisters unresolved <paramref name="target"/> in <paramref name="reference"/> of <paramref name="parent"/>.
    public IReferenceTarget UnregisterUnresolvedReference(IWritableNode parent, Feature reference, IReferenceTarget target)
    {
        LogUnregister(parent, reference, target);
        _unresolvedReferences.Remove((parent, reference, target));
        return target;
    }

    protected virtual void LogRegister(IWritableNode parent, Feature reference, IReferenceTarget target) =>
        Log($"Registering {target} in {parent.GetId()}.{reference}");

    protected virtual void LogUnregister(IWritableNode parent, Feature reference, IReferenceTarget target) =>
        Log($"Unregistering {target} in {parent.GetId()}.{reference}");

    protected virtual void LogResolve(IWritableNode parent, Feature reference, IReferenceTarget target) =>
        Log($"Resolving {target} in {parent.GetId()}.{reference}");

    protected virtual void Log(string message) =>
        Debug.WriteLine(message);
}