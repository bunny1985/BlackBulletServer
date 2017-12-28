using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;


namespace NotificationBackend.Infrastrucuture.WebSockets
{
    public class WebSocketConnectionManager
    {
        public class SocketIdentification{
            public string Id { get; set; }
            public string Tag { get; set; }
        }

        private ConcurrentDictionary<SocketIdentification,WebSocket> _sockets = new ConcurrentDictionary<SocketIdentification,  WebSocket> ();


        public WebSocket GetSocketById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key.Id == id).Value;
        }
        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key.Id;
        }

        public IEnumerable<WebSocket> GetSockestByTag(string tag)
        {
            return _sockets.Where(p => p.Key.Tag == tag).Select(e => e.Value);
        }
        public ConcurrentDictionary<SocketIdentification, WebSocket> GetAll()
        {
            return _sockets;
        }


        
        
        public void AddSocket(WebSocket socket , string tag )
        {
            var identity = new SocketIdentification(){Id = CreateConnectionId() , Tag =tag};
             _sockets.TryAdd(identity, socket);
        }

        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            var sockedKey = _sockets.First(s => s.Key.Id==id).Key;
            _sockets.TryRemove(sockedKey, out socket);
            

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, 
                                    statusDescription: "Closed by the WebSocketManager", 
                                    cancellationToken: CancellationToken.None);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}