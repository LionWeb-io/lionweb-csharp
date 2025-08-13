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

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.M1;
using Core.M3;
using Message;

public class LionWebTestRepository(
    LionWebVersions lionWebVersion,
    List<Language> languages,
    string name,
    IForest forest,
    IRepositoryConnector<IDeltaContent> connector)
    : LionWebRepository(lionWebVersion, languages, name, forest, connector)
{
    public const int _sleepInterval = 100;

    #region message received count

    private long _messageReceivedCount;
    public long MessageReceivedCount => Interlocked.Read(ref _messageReceivedCount);
    private void IncrementMessageReceivedCount() => Interlocked.Increment(ref _messageReceivedCount);
    public long WaitReceivedCount { get; set; }


    private void WaitForReceivedCount(long count)
    {
        while (MessageReceivedCount < count)
        {
            Log($"{nameof(MessageReceivedCount)}: {MessageReceivedCount} vs. {nameof(count)}: {count}");
            Thread.Sleep(_sleepInterval);
        }
    }

    /// Wait until <paramref name="delta"/> <i>more</i> messages than at the last call have been received.
    /// Counts any kind of <see cref="IDeltaContent">delta message</see>.
    public void WaitForReceived(int delta) =>
        WaitForReceivedCount(WaitReceivedCount += delta);

    /// <inheritdoc />
    protected override Task Receive(IMessageContext<IDeltaContent> messageContext)
    {
        IncrementMessageReceivedCount();
        return base.Receive(messageContext);
    }

    #endregion
    
    #region message sent count

    private long _messageSentCount;
    public long MessageSentCount => Interlocked.Read(ref _messageSentCount);
    private void IncrementMessageSentCount() => Interlocked.Increment(ref _messageSentCount);
    public long WaitSentCount { get; set; }
    
    private void WaitForSentCount(long count)
    {
        while (MessageSentCount < count)
        {
            Log($"{nameof(MessageSentCount)}: {MessageSentCount} vs. {nameof(count)}: {count}");
            Thread.Sleep(_sleepInterval);
        }
    }

    /// Wait until <paramref name="delta"/> <i>more</i> messages than at the last call have been sent.
    /// Counts any kind of <see cref="IDeltaContent">delta message</see>.
    public void WaitForSent(int delta) =>
        WaitForSentCount(WaitSentCount += delta);

    
    protected override Task Send(IClientInfo clientInfo, IDeltaContent deltaContent)
    {
        var result = base.Send(clientInfo, deltaContent);
        IncrementMessageSentCount();
        return result;
    }

    protected override Task SendAll(IDeltaContent deltaContent)
    {
        var result = base.SendAll(deltaContent);
        IncrementMessageSentCount();
        return result;
    }

    #endregion
    
    
}