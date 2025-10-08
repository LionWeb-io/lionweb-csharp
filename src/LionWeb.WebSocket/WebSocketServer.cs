namespace LionWeb.WebSocket;

using Core;
using Core.M3;
using Protocol.Delta;
using Protocol.Delta.Message;
using Protocol.Delta.Repository;
using System.Net;
using System.Net.WebSockets;
using System.Text;

public class WebSocketServer
{
    private const int BufferSize = 0x10000;
    public const string ServerStartedMessage = "Repository started.";

    private static string IpAddress { get; set; } = "localhost";

    public LionWebVersions LionWebVersion;
    public required List<Language> Languages { get; init; }
    public IDeltaRepositoryConnector Connector => _repositoryConnector;

    private readonly DeltaSerializer _deltaSerializer;
    private readonly IDeltaRepositoryConnector _repositoryConnector;

    private int _nextParticipationId = 0;

    private HttpListener? _listener;

    public WebSocketServer(LionWebVersions lionWebVersion)
    {
        LionWebVersion = lionWebVersion;
        _deltaSerializer = new DeltaSerializer();
        _repositoryConnector = new DeltaRepositoryConnector(lionWebVersion);
    }

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

    private async Task ProcessWebSocketRequest(HttpListenerContext context)
    {
        HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
        WebSocket socket = webSocketContext.WebSocket;
        var clientInfo = new ClientInfo() { ParticipationId = GetNextParticipationId() };
        _repositoryConnector.AddClient(clientInfo, new WebSocketDeltaRepositoryClient(socket));

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
                    _repositoryConnector.ReceiveFromClient(new DeltaMessageContext(clientInfo,
                            _deltaSerializer.Deserialize<IDeltaContent>(receivedMessage)));
                    break;
                }
                case WebSocketMessageType.Close:
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "",
                        CancellationToken.None);
                    _repositoryConnector.RemoveClient(clientInfo);
                    break;
            }
        }

        _repositoryConnector.RemoveClient(clientInfo);
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