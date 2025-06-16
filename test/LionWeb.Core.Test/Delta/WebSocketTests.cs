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

namespace LionWeb.Core.Test.Delta;

using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using Listener;
using M1;
using M1.Event;
using M1.Event.Partition;
using M3;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

[TestClass]
public class WebSocketTests
{
    [TestMethod, Timeout(3000)]
    public async Task Communication()
    {
        var server = new WebSocketServer();
        server.Received += (sender, msg) => Console.WriteLine($"server received: {msg}");

        var clientA = new WebSocketClient("A");
        clientA.Received += (sender, msg) => Console.WriteLine($"client A received: {msg}");

        var clientB = new WebSocketClient("B");
        clientB.Received += (sender, msg) => Console.WriteLine($"client B received: {msg}");

        var ipAddress = "localhost";
        var port = 42424;
        await server.StartServer(ipAddress, port);
        await clientA.ConnectToServer($"ws://{ipAddress}:{port}");
        await clientB.ConnectToServer($"ws://{ipAddress}:{port}");
        await server.Send("hello from server");
        await clientA.Send("hello from client A");
        await server.Send("bye from server");
        Thread.Sleep(100);
    }

    static IVersion2024_1 lionWebVersion = LionWebVersions.v2024_1;
    static List<Language> languages = [ShapesLanguage.Instance, lionWebVersion.BuiltIns, lionWebVersion.LionCore];

    [TestMethod, Timeout(3000)]
    public async Task Model()
    {
        var serverNode = new Geometry("a");
        var server = new WebSocketServer();
        {
            var receiverServer = new Receiver("server", serverNode, true);
            receiverServer.Send(s => server.Send(s));
            server.Received += (sender, msg) => receiverServer.Receive(msg);
        }


        var clientAClone = (Geometry)new SameIdCloner([serverNode]).Clone()[serverNode];
        var clientA = new WebSocketClient("A");
        var receiverA = new Receiver("client A", clientAClone);
        {
            receiverA.Send(s => clientA.Send(s));
            clientA.Received += (sender, msg) => receiverA.Receive(msg);
        }

        var clientBClone = (Geometry)new SameIdCloner([serverNode]).Clone()[serverNode];
        var clientB = new WebSocketClient("B");
        var receiverB = new Receiver("client B", clientBClone);
        {
            receiverB.Send(s => clientB.Send(s));
            clientB.Received += (sender, msg) => receiverB.Receive(msg);
        }

        var ipAddress = "localhost";
        var port = 42424;
        await server.StartServer(ipAddress, port);
        await clientA.ConnectToServer($"ws://{ipAddress}:{port}");
        await clientB.ConnectToServer($"ws://{ipAddress}:{port}");

        serverNode.Documentation = new Documentation("documentation");

        while (receiverA.MessageCount < 1)
            Thread.Sleep(100);

        clientAClone.Documentation.Text = "hello there";
        
        while (receiverA.MessageCount < 1 || receiverB.MessageCount < 2)
            Thread.Sleep(100);

        AssertEquals([serverNode], [clientAClone]);
        AssertEquals([serverNode], [clientBClone]);
    }

    private class Receiver
    {
        private readonly string _name;
        private readonly Dictionary<NodeId, IReadableNode> _sharedNodeMap;
        private readonly DeltaProtocolPartitionEventReceiver _eventReceiver;
        private readonly DeltaSerializer _deltaSerializer;
        private readonly IPartitionPublisher _publisher;

        private long _messageCount;

        public long MessageCount => Interlocked.Read(ref _messageCount);

        public Receiver(string name, IPartitionInstance partition, bool replicateChanges = false)
        {
            _name = name;
            _sharedNodeMap = [];
            var partitionEventHandler = new PartitionEventHandler(null);
            DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages);
            Dictionary<CompressedMetaPointer, IKeyed>
                sharedKeyedMap = DeltaCommandToDeltaEventMapper.BuildSharedKeyMap(languages);
            _eventReceiver = new DeltaProtocolPartitionEventReceiver(
                partitionEventHandler,
                _sharedNodeMap,
                sharedKeyedMap,
                deserializerBuilder
            );
            var replicator = new PartitionEventReplicator(partition, _sharedNodeMap);
            replicator.ReplicateFrom(partitionEventHandler);
            _deltaSerializer = new DeltaSerializer();
            
            _publisher = replicateChanges ? partition.GetPublisher() : replicator;
        }

        public void Send(Action<string> action)
        {
            var commandToEventMapper = new PartitionEventToDeltaEventMapper(new ConstParticipationIdProvider(), new EventSequenceNumberProvider(), lionWebVersion);

            _publisher.Subscribe<IPartitionEvent>((sender, partitionEvent) =>
            {
                var @event = commandToEventMapper.Map(partitionEvent);

                Console.WriteLine($"{_name} sending event: {@event}");
                var deltaSerializer = new DeltaSerializer();
                action(deltaSerializer.Serialize(@event));
            });
        }

        public void Receive(string msg)
        {
            try
            {
                Console.WriteLine($"{_name} received: {msg}");
                var @event = _deltaSerializer.Deserialize<IDeltaEvent>(msg);
                _eventReceiver.Receive(@event);
                Interlocked.Increment(ref _messageCount);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
}

class WebSocketServer
{
    public const int BUFFER_SIZE = 0x10000;

    private readonly ConcurrentDictionary<WebSocket, byte> _openSockets = [];

    public event EventHandler<string> Received;

    public async Task StartServer(string ipAddress, int port)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add($"http://{ipAddress}:{port}/");
        listener.Start();

        Console.WriteLine("Server started. Waiting for connections...");

        Task.Run(async () =>
        {
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    ProcessWebSocketRequest(context);
                } else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        });
    }

    public async Task Send(string msg)
    {
        foreach ((WebSocket socket, var _) in _openSockets)
        {
            await socket.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
    }

    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
        WebSocket socket = webSocketContext.WebSocket;
        _openSockets.TryAdd(socket, 1);

        Console.WriteLine($"WebSocket connection accepted: {context.Request.RemoteEndPoint}");

        // Handle incoming messages
        byte[] buffer = new byte[BUFFER_SIZE];
        while (socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result =
                await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Received?.Invoke(this, receivedMessage);
            } else if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "",
                    CancellationToken.None);
                _openSockets.TryRemove(socket, out _);
            }
        }

        _openSockets.TryRemove(socket, out _);
    }
}

class WebSocketClient(string name)
{
    private readonly ClientWebSocket _clientWebSocket = new ClientWebSocket();

    public event EventHandler<string> Received;

    public async Task ConnectToServer(string serverUri)
    {
        await _clientWebSocket.ConnectAsync(new Uri(serverUri), CancellationToken.None);

        Console.WriteLine($"Client {name} Connected to the server: {serverUri}");

        Task.Run(async () =>
        {
            // Receive messages from the server
            byte[] receiveBuffer = new byte[WebSocketServer.BUFFER_SIZE];
            while (_clientWebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result =
                    await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                    Received?.Invoke(this, receivedMessage);
                }
            }
        });
    }

    public async Task Send(string msg)
    {
        await _clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg)), WebSocketMessageType.Text,
            true, CancellationToken.None);
    }
}

public class ConstParticipationIdProvider : IParticipationIdProvider
{
    public string ParticipationId => "abc";
}

public class EventSequenceNumberProvider : IEventSequenceNumberProvider
{
    private long next = 0;
    public long Create() => ++next;
}