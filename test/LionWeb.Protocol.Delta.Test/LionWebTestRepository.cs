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
    
    private long _messageCount;
    public long MessageCount => Interlocked.Read(ref _messageCount);
    private void IncrementMessageCount() => Interlocked.Increment(ref _messageCount);
    public long WaitCount { get; set; }


    private void WaitForCount(long count)
    {
        while (MessageCount < count)
        {
            Log($"{nameof(MessageCount)}: {MessageCount} vs. {nameof(count)}: {count}");
            Thread.Sleep(_sleepInterval);
        }
    }

    /// Wait until <paramref name="delta"/> <i>more</i> messages than at the last call have been received.
    /// Counts any kind of <see cref="IDeltaContent">delta message</see>.
    public void WaitForReceived(int delta) =>
        WaitForCount(WaitCount += delta);

    /// <inheritdoc />
    protected override Task Receive(IMessageContext<IDeltaContent> messageContext)
    {
        IncrementMessageCount();
        return base.Receive(messageContext);
    }
}