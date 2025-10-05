namespace LionWeb.WebSocket;

using Core;
using Core.M3;
using Core.Notification;
using Protocol.Delta;
using Protocol.Delta.Message;
using Protocol.Delta.Message.Event;
using Protocol.Delta.Repository;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

public class WebSocketServer : IDeltaRepositoryConnector
{
    private const int BufferSize = 0x10000;
    public const string ServerStartedMessage = "Repository started.";

    private static string IpAddress { get; set; } = "localhost";

    public LionWebVersions LionWebVersion;
    public required List<Language> Languages { get; init; }

    private readonly DeltaSerializer _deltaSerializer = new();
    private readonly NotificationToDeltaEventMapper _mapper;

    private readonly ConcurrentDictionary<IClientInfo, WebSocket> _knownClients = [];
    private int _nextParticipationId = 0;

    private HttpListener? _listener;

    public WebSocketServer(LionWebVersions lionWebVersion)
    {
        LionWebVersion = lionWebVersion;
        _mapper = new(new ExceptionParticipationIdProvider(), lionWebVersion);
    }

    /// <inheritdoc />
    public event EventHandler<IMessageContext<IDeltaContent>>? ReceiveFromClient;

    public void StartServer(string ipAddress, int port)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://{ipAddress}:{port}/");
        _listener.Start();

        Log(ServerStartedMessage + " Waiting for connections...");

        // do NOT await!
        Task.Run(async () =>
        {
            while (true)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    // do NOT await!
                    ProcessWebSocketRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        });
    }

    public void Stop()
    {
        if (_listener == null)
            return;

        _listener.Stop();
    }

    /// <inheritdoc />
    public async Task SendToAllClients(IDeltaContent content)
    {
        foreach ((var clientInfo, WebSocket socket) in _knownClients)
        {
            var encoded = Encode(_deltaSerializer.Serialize(UpdateSequenceNumber(content, clientInfo)));
            // Log($"XXServer: sending to {clientInfo} message: {content.GetType()}");
            await socket.SendAsync(encoded, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    private static IDeltaContent UpdateSequenceNumber(IDeltaContent content, IClientInfo clientInfo)
    {
        if (content is IDeltaEvent deltaEvent)
        {
            deltaEvent.SequenceNumber = clientInfo.IncrementAndGetSequenceNumber();
        }

        return content;
    }

    /// <inheritdoc />
    public IDeltaContent Convert(INotification notification) =>
        _mapper.Map(notification);

    private static byte[] Encode(string msg) =>
        Encoding.UTF8.GetBytes(msg);


    /// <inheritdoc />
    public async Task SendToClient(IClientInfo clientInfo, IDeltaContent content) =>
        await Send(clientInfo, _deltaSerializer.Serialize(UpdateSequenceNumber(content, clientInfo)));

    private async Task Send(IClientInfo clientInfo, string msg)
    {
        if (_knownClients.TryGetValue(clientInfo, out var socket))
        {
            var encoded = Encode(msg);
            await socket.SendAsync(encoded, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
        WebSocket socket = webSocketContext.WebSocket;
        var clientInfo = new ClientInfo() { ParticipationId = GetNextParticipationId() };
        _knownClients.TryAdd(clientInfo, socket);

        Log($"WebSocket connection accepted: {context.Request.RemoteEndPoint}");

        // Handle incoming messages
        byte[] buffer = new byte[BufferSize];
        while (socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result =
                await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ReceiveFromClient?.Invoke(this,
                        new DeltaMessageContext(clientInfo,
                            _deltaSerializer.Deserialize<IDeltaContent>(receivedMessage)));
                    break;
                }
                case WebSocketMessageType.Close:
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "",
                        CancellationToken.None);
                    _knownClients.TryRemove(clientInfo, out _);
                    break;
            }
        }

        _knownClients.TryRemove(clientInfo, out _);
    }

    private string GetNextParticipationId()
    {
        lock (this)
        {
            return "participation" + _nextParticipationId++;
        }
    }

    private static void Log(string message, bool header = false) =>
        Console.WriteLine(header
            ? $"{ILionWebRepository.HeaderColor_Start}{message}{ILionWebRepository.HeaderColor_End}"
            : message);
}