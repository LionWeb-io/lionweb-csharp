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

namespace LionWeb.Core.Notification;

/// <summary>
/// Extensions for <see cref="INotification"/>.
/// </summary>
public static class INotificationExtensions
{
    /// <summary>
    /// Recursively collects <paramref name="notification"/> and all nested notifications.
    /// </summary>
    /// <param name="notification">Notification to start collecting on.</param>
    /// <param name="includeSelf">Whether <paramref name="notification"/> should be included in the result; defaults to <see langword="true"/>.</param>
    /// <returns>All nested notifications for <paramref name="notification"/> in top-down, depth-first order.</returns>
    /// <seealso cref="CompositeNotification"/>
    public static List<INotification> CollectNested(this INotification notification, bool includeSelf = true)
    {
        List<INotification> result = [];
        CollectNested(notification, result);

        if (!includeSelf)
            result.Remove(notification);

        return result;
    }

    private static void CollectNested(INotification notification, List<INotification> nested)
    {
        if (nested.Contains(notification))
            return;
        
        nested.Add(notification);
        
        if (notification is CompositeNotification composite)
            foreach (var part in composite.Parts)
            {
                CollectNested(part, nested);
            }
    }
}