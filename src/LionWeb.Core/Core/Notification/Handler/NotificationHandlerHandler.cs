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

internal class NotificationHandlerHandler<TSubscribedNotification>(EventHandler<TSubscribedNotification> handler)
    : INotificationHandler<TSubscribedNotification> where TSubscribedNotification : INotification
{
    public void Dispose()
    {
    }

    public required string NotificationHandlerId { get; init; }

    /// <inheritdoc />
    public bool CanReceive(params Type[] messageTypes) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void Receive(TSubscribedNotification message) =>
        handler.Invoke(null, message);

    /// <inheritdoc />
    public void Send(TSubscribedNotification message) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void Subscribe<TReceiveTo, TSendTo>(IHandler<TReceiveTo, TSendTo> receiver) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    void IHandler.Unsubscribe<T>(IHandler receiver) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void PrintAllReceivers(List<IHandler> alreadyPrinted, string indent = "")
    {
        Console.WriteLine($"{indent}{this.GetType().Name}");
        if (IHandler.RecursionDetected(this, alreadyPrinted, indent))
            return;

        if (handler is IHandler p)
            p.PrintAllReceivers(alreadyPrinted, indent + "  ");
        else
            Console.WriteLine($"{indent}{handler}");
    }
}