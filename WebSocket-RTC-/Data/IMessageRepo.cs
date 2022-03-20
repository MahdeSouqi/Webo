using System;
using System.Collections.Generic;
using WebSocketServer.Model;

namespace WebSocketServer.Data
{
    public interface IMessageRepo
    {
        public void AddMessage(Message message);

        public IEnumerable<Guid> GetAllFollower(Guid vetId);


        public bool SaveChanges();

        public IEnumerable<Message> GetAllMessagesBySenderReceiverId(Guid senderId, Guid receiverId);
    }
}