using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationBackend.Infrastrucuture.WebSockets
{
    public abstract class WebSocketHandler
    {
        protected WebSocketConnectionManager _webSocketConnectionManager { get; set; }

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            _webSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket, string tag)
        {
            _webSocketConnectionManager.AddSocket(socket , tag);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await _webSocketConnectionManager.RemoveSocket(_webSocketConnectionManager.GetId(socket));
        }


        public async Task SendMessageToAllSocketsWithTagAsync(string tag , string message)
        {
            var sockets = _webSocketConnectionManager.GetSockestByuserName(tag);
            foreach (var socket in sockets)
            {
                try{
                    await  SendMessageAsync(socket , message);
                }catch(Exception){}
                
            }
            
            
                    
            
        }
        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if(socket.State != WebSocketState.Open)
                return;
            try{
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                  offset: 0, 
                                                                  count: message.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);          
            }catch(Exception){
                await _webSocketConnectionManager.RemoveSocket(_webSocketConnectionManager.GetId(socket));
            }
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            await SendMessageAsync(_webSocketConnectionManager.GetSocketById(socketId), message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach(var pair in _webSocketConnectionManager.GetAll())
            {
                if(pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}