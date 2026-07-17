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

using LionWeb.Core.Notification;

namespace LionWeb.Core.Test.Notification;

/// <summary>
/// Extension methods for asserting on collected notifications from <see cref="NotificationObserver"/>.
/// </summary>
public static class NotificationListAssertExtensions
{
    /// <summary>
    /// Asserts that exactly <paramref name="count"/> notifications of type <typeparamref name="T"/> were received,
    /// and returns them as a typed list.
    /// </summary>
    public static List<T> OfType<T>(this NotificationObserver observer, Index count) where T : INotification
    {
        var typed = observer.Notifications.OfType<T>().ToList();
        Assert.AreEqual(count, typed.Count,
            $"Expected {count} notification(s) of type {typeof(T).Name}, but got {typed.Count}. " +
            $"All notifications: [{string.Join(", ", observer.Notifications.Select(n => n.GetType().Name))}]");
        return typed;
    }

    /// <summary>
    /// Asserts that the observer received no notifications at all.
    /// </summary>
    public static void AssertEmpty(this NotificationObserver observer) =>
        Assert.AreEqual(0, observer.Count,
            $"Expected no notifications, but got {observer.Count}: [{string.Join(", ", observer.Notifications.Select(n => n.GetType().Name))}]");

    /// <summary>
    /// Asserts that the observer received no notifications of type <typeparamref name="T"/>.
    /// </summary>
    public static void AssertNone<T>(this NotificationObserver observer) where T : INotification
    {
        var typed = observer.Notifications.OfType<T>().ToList();
        Assert.IsEmpty(typed, $"Expected no notifications of type {typeof(T).Name}, but got {typed.Count}.");
    }
}
