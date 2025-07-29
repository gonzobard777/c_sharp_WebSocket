using Microsoft.AspNetCore.Mvc;
using WSocket.Messaging;

namespace WSocket.Controllers;

public class WebSocketController(MessageHandler messageHandler) : ControllerBase
{
    public MessageHandler MessageHandler { get; } = messageHandler;

    [Route("/ws")]
    public async Task Get(CancellationToken cancellationToken)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var connection = new ClientConnection(webSocket, MessageHandler);
            MessageHandler.AddClientConnection(connection);
            try
            {
                await connection.Run(cancellationToken);
            }
            finally
            {
                MessageHandler.RemoveClientConnection(connection);
            }
        }
        else
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
}