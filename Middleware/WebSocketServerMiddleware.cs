using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebSocketServer.Data;
using WebSocketServer.DTO;
using WebSocketServer.Model;

namespace WebSocketServer.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly WebSocketServerConnectionManager _manager;
        

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager )
        {
            
            _next = next;
            _manager = manager;
          
        }

        public async Task InvokeAsync(HttpContext context,IMessageRepo repo,IMapper mapper)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                
                
                await Receive(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine($"Receive->Text");
                        var jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var message = JsonConvert.DeserializeObject<dynamic>(jsonMessage);
                        Console.WriteLine(message);
                        string msgText = message.MessageText.ToString();
                        if (msgText.Equals("SenderId"))
                        {
                            Guid senderId = message.SenderId;
                            _manager.AddSocket(message.SenderId.ToString(),webSocket);
                            var allFollowerId = repo.GetAllFollower(senderId);
                            List<UserAndMessagesDto> userAndMessagesDto = new List<UserAndMessagesDto>();

                            foreach (var userId in allFollowerId)
                            {
                               
                                var msgAndId = new UserAndMessagesDto
                                {
                                    UserId = userId,
                                    Message = mapper.Map<IEnumerable<MessageReadDto>>(repo.GetAllMessagesBySenderReceiverId(senderId,userId))
                                };
                                userAndMessagesDto.Add(msgAndId);
                            }
                           
                            foreach (var msg in userAndMessagesDto)
                            {
                                
                                var toSend = JsonConvert.SerializeObject(msg);
                                await webSocket.SendAsync(Encoding.UTF8.GetBytes(toSend),
                                    WebSocketMessageType.Text, true, CancellationToken.None); 
                            }
                        }
                        else
                        {
                            await RouteJSONMessageAsync(jsonMessage, repo);
                            return;
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        string id = _manager.GetAllSockets().FirstOrDefault(s => s.Value == webSocket).Key;
                        Console.WriteLine($"Receive->Close on: " + id);

                        // WebSocket sock;
                        //  _manager.GetAllSockets().TryRemove(id, out sock);
                        // Console.WriteLine("Managed Connections: " + _manager.GetAllSockets().Count.ToString());

                        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                        return;
                    }
                });
            }
            else
            {
                Console.WriteLine("Hello from 2nd Request Delegate - No WebSocket");
                await _next(context);
            }
        }

        private async Task RouteJSONMessageAsync(string message,IMessageRepo repo)
        {

            var messageDto = JsonConvert.DeserializeObject<dynamic>(message);
            Message mappedMessage = new Message
            {
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId,
                MessageText = messageDto.MessageText.ToString(),
                Date = messageDto.Date
                    
            };
            
            Console.WriteLine("To: " + mappedMessage.ReceiverId);
            Guid guidOutput;

             if (Guid.TryParse(mappedMessage.ReceiverId.ToString(), out guidOutput))
             {
                Console.WriteLine("Targeted");
                var sock = _manager.GetAllSockets().FirstOrDefault(s => 
                    s.Key == mappedMessage.ReceiverId.ToString());
                if (sock.Value != null)
                {
                    if (sock.Value.State == WebSocketState.Open)// if the receiver online
                    {
                        mappedMessage.HaveSeen = true;
                        repo.AddMessage(mappedMessage);
                        repo.SaveChanges();
                        await sock.Value.SendAsync(Encoding.UTF8.GetBytes(mappedMessage.MessageText.ToString()),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else if(sock.Value.State==WebSocketState.Closed)//receiver offline
                    {
                        mappedMessage.HaveSeen = false; 
                        repo.AddMessage(mappedMessage);
                        repo.SaveChanges();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Recipient");
                }
             }
             

        }     
        

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}