using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketServer
{
    public class WebSocketServerConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public void AddSocket(string connectionId,WebSocket socket)
        {
           
            _sockets.TryAdd(connectionId, socket);
           
        }

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }
    }
}