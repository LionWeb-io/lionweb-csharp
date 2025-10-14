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

namespace LionWeb.WebSocket;

using Core;
using Core.Notification;
using Protocol.Delta.Client;
using Protocol.Delta.Message;

public class WebSocketDeltaClientConnector : IDeltaClientConnector
{
    private readonly Func<IDeltaContent, Task> _sender;
    private readonly NotificationToDeltaCommandMapper _mapper;

    public WebSocketDeltaClientConnector(LionWebVersions lionWebVersion, Func<IDeltaContent, Task> sender)
    {
        _sender = sender;
        _mapper = new(new CommandIdProvider(), lionWebVersion);
    }

    /// <inheritdoc />
    public event EventHandler<IDeltaContent>? ReceivedFromRepository;

    /// <inheritdoc />
    public async Task SendToRepository(IDeltaContent content) =>
        await _sender(content);

    /// <inheritdoc />
    public IDeltaContent Convert(INotification notification)
        => _mapper.Map(notification);

    public void ReceiveFromRepository(IDeltaContent content) =>
        ReceivedFromRepository?.Invoke(this, content);
}