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

namespace LionWeb.Protocol.Delta.Test.Client;

using Core;
using Core.Notification;
using Delta.Client;
using Message;

internal class DeltaClientConnector : IDeltaClientConnector
{
    private readonly Action<IDeltaContent> _sender;
    private readonly NotificationToDeltaCommandMapper _mapper;

    public DeltaClientConnector(LionWebVersions lionWebVersion, Action<IDeltaContent> sender)
    {
        _sender = sender;
        _mapper = new NotificationToDeltaCommandMapper(new CommandIdProvider(), lionWebVersion);
    }

    public Task SendToRepository(IDeltaContent content)
    {
        _sender(content);
        return Task.CompletedTask;
    }

    public event EventHandler<IDeltaContent>? ReceiveFromRepository;

    public void ReceiveMessageFromRepository(IDeltaContent context) => ReceiveFromRepository?.Invoke(null, context);
    public IDeltaContent Convert(INotification notification) => _mapper.Map(notification);
}