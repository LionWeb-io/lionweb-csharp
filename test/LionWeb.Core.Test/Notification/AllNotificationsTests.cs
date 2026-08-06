// Copyright 2026 TRUMPF Laser SE and other contributors
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
using M3;
using System.Collections;
using System.Reflection;

[TestClass]
public class AllNotificationsTests : AllNotificationTestsBase
{
    [TestMethod]
    [DynamicData(nameof(CollectAllNotifications), DynamicDataSourceType.Method)]
    public void AffectedNodes(INotification notification)
    {
        var containedNodes = UnwrapNodes(notification).ToList();

        CollectionAssert.AreEquivalent(containedNodes, notification.AffectedNodes.ToList(), $"""

             Expected: {string.Join(",", containedNodes.Select(n => n.GetId()).Order())}
             Actual  : {string.Join(",", notification.AffectedNodes.Select(n => n.GetId()).Order())}
             """);
    }

    private static IEnumerable<IReadableNode> UnwrapNodes(INotification notification)
    {
        var fieldInfos = AllBaseTypes(notification.GetType())
        .SelectMany(t => t.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance))
        .ToList();
        return fieldInfos
            .Select(f => f.GetValue(notification))
            .SelectMany(v => v switch
            {
                IKeyed => [],
                IReadableNode n => [n],
                string => [],
                List<INotification> p => p.SelectMany(UnwrapNodes),
                IEnumerable e => e.Cast<IReadableNode>(),
                IReferenceTarget t => [t.Target],
                _ => []
            });
    }
    
    private static IEnumerable<Type> AllBaseTypes(Type t)
    {
        if (t.BaseType is not null)
            return [t, .. AllBaseTypes(t.BaseType)];

        return [t];
    }
}