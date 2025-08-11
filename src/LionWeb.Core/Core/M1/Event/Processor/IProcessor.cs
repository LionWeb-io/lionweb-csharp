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

namespace LionWeb.Core.M1.Event.Processor;

/// A member in a directed graph that sends messages.
/// Each member is <see cref="Connect{TReceiveFrom,TConnected,TSendFrom}">connected</see>
/// <i>from</i> one or more <i>preceding</i> processors, and
/// <i>to</i> one or more <i>following</i> processors.
///
/// <para>
/// <i>Inbound</i> processors have no <i>preceding</i> part.
/// They receive their messages from outside the processor graph via <see cref="IProcessor{TReceive,TSend}.Receive"/>. 
/// </para>
///
/// <para>
/// Upon <see cref="IProcessor{TReceive,TSend}.Receive">receiving</see> a message,
/// a processor can choose to <see cref="IProcessor{TReceive,TSend}.Send"/> the unmodified, modified, or a new message
/// to its <i>following</i> processors.
/// A processor can also suppress an incoming message, i.e. not send the message to its <i>following</i> processors. 
/// </para>
public interface IProcessor
{
    protected string ProcessorId { get; }
    
    /// All messages <see cref="IProcessor{TReceive,TSend}.Send">sent</see> by <paramref name="from"/>
    /// will be <see cref="IProcessor{TReceive,TSend}.Receive">received</see> by <paramref name="to"/>. 
    public static void Connect<TReceiveFrom, TConnected, TSendFrom>(
        IProcessor<TReceiveFrom, TConnected> from,
        IProcessor<TConnected, TSendFrom> to) =>
        from.Subscribe(to);

    /// Unsubscribes <paramref name="receiver"/> from this.
    /// For internal use only -- each processor should unsubscribe itself from all <i>preceding</i> processors on disposal.
    protected internal void Unsubscribe<T>(IProcessor receiver);
    
    protected internal void PrintAllReceivers(List<IProcessor> alreadyPrinted, string indent = "");

    protected static bool RecursionDetected(IProcessor self, List<IProcessor> alreadyPrinted, string indent)
    {
        try
        {
            if (!alreadyPrinted.Contains(self))
                return false;

            Console.WriteLine($"{indent}Recursion ^^");
            return true;

        } finally
        {
            alreadyPrinted.Add(self);
        }
    }
}

/// A processor that receives <typeparamref name="TReceive"/> messages and sends <typeparamref name="TSend"/> messages.
public interface IProcessor<in TReceive, in TSend> : IProcessor, IDisposable
{
    /// Whether anybody would receive any of the <paramref name="messageTypes"/> events.
    /// Useful for returning eagerly from complex logic to calculate the event contents.
    /// <value>
    ///     <c>true</c> if someone would receive any of the <paramref name="messageTypes"/> events; <c>false</c> otherwise.
    /// </value>
    public bool CanReceive(params Type[] messageTypes);
    
    /// This processor receives <paramref name="message"/>.
    /// Call this on <i>inbound</i> processors (i.e. processors that get messages from outside the processor chain).
    public void Receive(TReceive message);
    
    /// This processor wants to send <paramref name="message"/>.
    /// Only this processor should use this method.
    protected void Send(TSend message);

    /// Subscribes <paramref name="receiver"/> to this, <paramref name="receiver"/>
    /// <see cref="Receive">receives</see> all messages <see cref="Send">sent</see> by this processor.
    /// For internal use only, use <see cref="IProcessor.Connect{TReceiveFrom,TForward,TSendFrom}"/>.
    internal void Subscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver);
}