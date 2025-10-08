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

using Protocol.Delta;
using Protocol.Delta.Message;
using Protocol.Delta.Repository;
using System.Net.WebSockets;
using System.Text;

public class WebSocketDeltaRepositoryClient(WebSocket webSocket) : IDeltaRepositoryClient
{
    private readonly DeltaSerializer _deltaSerializer = new();

    /// <inheritdoc />
    public async Task SendToClient(IDeltaContent content) =>
        await webSocket.SendAsync(Encode(_deltaSerializer.Serialize(content)), WebSocketMessageType.Text, true,
            CancellationToken.None);
    
    private static byte[] Encode(string msg) =>
        Encoding.UTF8.GetBytes(msg);
}