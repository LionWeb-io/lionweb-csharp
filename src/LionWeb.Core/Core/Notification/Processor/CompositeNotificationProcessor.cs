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

namespace LionWeb.Core.Notification.Processor;

/// Composes two or more <see cref="INotificationProcessor{TNotification}"/>s.
///
/// Every message this processor <see cref="Receive">receives</see>
/// is forwarded to the first <see cref="_notificationProcessors">component</see>.
/// Each component is connected to the next component.
/// The last component <see cref="IProcessor{TReceive,TSend}.Send">sends</see> to
/// this processor's <i>following</i> processors.
public class CompositeNotificationProcessor<TNotification> : INotificationProcessor<TNotification>
    where TNotification : class, INotification
{
    private readonly INotificationProcessor<TNotification> _firstProcessor;
    private readonly INotificationProcessor<TNotification> _lastProcessor;
    private readonly IEnumerable<INotificationProcessor<TNotification>> _notificationProcessors;
    private readonly object? _sender;

    public CompositeNotificationProcessor(List<INotificationProcessor<TNotification>> notificationProcessors,
        object? sender)
    {
        _notificationProcessors = notificationProcessors;
        _sender = sender;
        if (notificationProcessors.Count < 2)
            throw new ArgumentException(
                $"{nameof(CompositeNotificationProcessor<TNotification>)} must get at least 2 processors");

        _firstProcessor = notificationProcessors[0];
        _lastProcessor = notificationProcessors[^1];

        var enumerator = notificationProcessors.GetEnumerator();
        enumerator.MoveNext();
        var previous = enumerator.Current;
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            IProcessor.Connect(previous, current);

            previous = current;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (INotificationProcessor<TNotification> processor in _notificationProcessors.Reverse())
        {
            processor.Dispose();
        }
    }

    /// <inheritdoc />
    public string ProcessorId =>
        _sender?.ToString() ?? GetType().Name;

    /// <inheritdoc />
    public void Receive(TNotification message) =>
        _firstProcessor.Receive(message);

    /// <inheritdoc />
    public bool CanReceive(params Type[] messageTypes) =>
        _firstProcessor.CanReceive(messageTypes);

    void IProcessor<TNotification, TNotification>.Send(TNotification message) =>
        throw new ArgumentException("Should never be called");

    void IProcessor<TNotification, TNotification>.Subscribe<TReceiveTo, TSendTo>(
        IProcessor<TReceiveTo, TSendTo> receiver) =>
        _lastProcessor.Subscribe(receiver);

    void IProcessor.Unsubscribe<T>(IProcessor receiver) =>
        _lastProcessor.Unsubscribe<T>(receiver);

    /// <inheritdoc />
    public void PrintAllReceivers(List<IProcessor> alreadyPrinted, string indent = "")
    {
        Console.WriteLine($"{indent}{this.GetType().Name}({_sender})");
        if (IProcessor.RecursionDetected(this, alreadyPrinted, indent))
            return;

        Console.WriteLine($"{indent}Members:");
        foreach (var processor in this._notificationProcessors)
        {
            processor.PrintAllReceivers(alreadyPrinted, indent + "  ");
        }
    }
}