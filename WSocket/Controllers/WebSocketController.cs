using Microsoft.AspNetCore.Mvc;
using WSocket.Messaging;

namespace WSocket.Controllers;

public class WebSocketController : ControllerBase
{
    private MessageHandler MessageHandler { get; }

    public WebSocketController(MessageHandler messageHandler)
    {
        MessageHandler = messageHandler;
    }

    [Route("/ws")]
    public async Task Get(CancellationToken cancellationToken)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var connection = new ClientConnection(webSocket, MessageHandler, cancellationToken);
            try
            {
                MessageHandler.AddConnection(connection);
                await connection.Run();
            }
            finally
            {
                MessageHandler.DisposeClientConnection(connection);
            }
        }
        else
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
}