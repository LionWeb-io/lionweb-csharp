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

internal class NativeEventNotificationHandler<TNotification>(EventHandler<TNotification> handler)
    : IReceivingNotificationHandler where TNotification : INotification
{
    public void Dispose()
    {
    }

    public required string NotificationHandlerId { get; init; }

    /// <inheritdoc />
    public bool Handles(params Type[] notificationTypes) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification) 
    {
        if (notification is TNotification n)
            handler.Invoke(correspondingHandler, n);
    }

    /// <inheritdoc />
    public void PrintAllReceivers(List<INotificationHandler> alreadyPrinted, string indent = "")
    {
        Console.WriteLine($"{indent}{this.GetType().Name}");
        if (INotificationHandler.RecursionDetected(this, alreadyPrinted, indent))
            return;

        if (handler is INotificationHandler p)
            p.PrintAllReceivers(alreadyPrinted, indent + "  ");
        else
            Console.WriteLine($"{indent}{handler}");
    }
}