using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace NotificationBackend.Infrastrucuture.WebSockets
{
    public class NotificationSocketHandler: WebSocketHandler
    {
        public NotificationSocketHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public async override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            await SendMessageAsync(socket , "Banned for sending a message to server - this is one way communication chanell. Bye :) " );
            await this.WebSocketConnectionManager.RemoveSocket(this.WebSocketConnectionManager.GetId(socket));
        }
    }
}