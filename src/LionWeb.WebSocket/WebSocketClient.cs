// Copyright 2025 LionWeb Project
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
// SPDX-FileCopyrightText: 2025 LionWeb Project
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.WebSocket;

using Core;
using Core.M3;
using Core.Notification;
using Protocol.Delta;
using Protocol.Delta.Client;
using Protocol.Delta.Message;
using System.Net.WebSockets;
using System.Text;

public class WebSocketClient : IDeltaClientConnector
{
    private const int BufferSize = 0x10000;

    public const string ClientStartedMessage = "Client started.";

    private static readonly IVersion2023_1 _lionWebVersion = LionWebVersions.v2023_1;

    private static readonly List<Language> _languages =
        [_lionWebVersion.BuiltIns, _lionWebVersion.LionCore];

    private readonly NotificationToDeltaCommandMapper _mapper;

    public WebSocketClient(string name)
    {
        _name = name;
        _mapper = new(new CommandIdProvider(), _lionWebVersion);
    }

    private async Task SignOn(LionWebTestClient lionWeb, RepositoryId repositoryId) =>
        await lionWeb.SignOn(repositoryId);


    private async Task SignOff(LionWebTestClient lionWeb) =>
        await lionWeb.SignOff();

    private readonly DeltaSerializer _deltaSerializer = new();
    private readonly ClientWebSocket _clientWebSocket = new ClientWebSocket();
    private readonly string _name;

    /// <inheritdoc />
    public event EventHandler<IDeltaContent>? ReceiveFromRepository;

    public async Task ConnectToServer(string ipAddress, int port) =>
        await ConnectToServer($"ws://{ipAddress}:{port}");

    public async Task ConnectToServer(string serverUri)
    {
        await _clientWebSocket.ConnectAsync(new Uri(serverUri), CancellationToken.None);

        Log($"{_name}: {ClientStartedMessage} Connected to the server: {serverUri}");

        Task.Run(async () =>
        {
            // Receive messages from the server
            byte[] receiveBuffer = new byte[BufferSize];
            while (_clientWebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result =
                    await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                    // Log($"XXClient: received message: {receivedMessage}");
                    var deserialized = _deltaSerializer.Deserialize<IDeltaContent>(receivedMessage);
                    // do NOT await
                    Task.Run(() => ReceiveFromRepository?.Invoke(this, deserialized));
                    // Log($"XXClient: processed message: {receivedMessage}");
                }
            }
        });
    }

    /// <inheritdoc />
    public async Task SendToRepository(IDeltaContent content) =>
        await Send(_deltaSerializer.Serialize(content));

    public async Task Send(string msg) =>
        await _clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg)), WebSocketMessageType.Text,
            true, CancellationToken.None);

    /// <inheritdoc />
    public IDeltaContent Convert(INotification notification)
        => _mapper.Map(notification);

    private static void Log(string message, bool header = false) =>
        Console.WriteLine(header
            ? $"{ILionWebClient.HeaderColor_Start}{message}{ILionWebClient.HeaderColor_End}"
            : message);
}