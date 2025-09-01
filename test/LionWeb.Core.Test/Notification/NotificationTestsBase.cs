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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Pipe;
using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using System.Reflection;
using Comparer = Core.Utilities.Comparer;

public abstract class NotificationTestsBase
{
    protected abstract Geometry CreateReplicator(Geometry node);

    protected Geometry Clone(Geometry node) =>
        (Geometry)new SameIdCloner([node]) { IncludingReferences = true }.Clone()[node];

    protected void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
}

public static class NotificationExtensions
{
    // TODO: Replace by proper ConnectTo() calls
    public static void Subscribe<T>(this INotificationSender sender, Action<object?, T> handler) => 
        sender.ConnectTo(new FilteredReceiver<T>(handler));
}

internal class FilteredReceiver<T>(Action<object?, T> handler) : INotificationReceiver
{
    public bool Handles(params Type[] notificationTypes) =>
        notificationTypes.Any(t => typeof(T).IsAssignableFrom(t));

    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        if (notification is T t)
            handler.Invoke(correspondingSender, t);
    }
} 