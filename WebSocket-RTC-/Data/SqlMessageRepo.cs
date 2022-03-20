using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketServer.Model;

namespace WebSocketServer.Data
{
    
        public class SqlMessageRepo : IMessageRepo
        {
            private readonly MessageContext _context;

            public SqlMessageRepo(MessageContext context)
            {
                _context = context;
            }
        
            public void AddMessage(Message message)
            {

                if (message == null)
                {
                    throw new ArgumentNullException();
                }

                _context.Messages.Add(message);
            }

            public IEnumerable<Guid> GetAllFollower(Guid vetId)
            {

                var userToVet = _context.Messages.Where(r => r.ReceiverId == vetId).ToList().Select(s=>s.SenderId);// i'm the receiver
                var vetToUser = _context.Messages.Where(s => s.SenderId == vetId).ToList().Select(r=>r.ReceiverId);//i'm the sender
                if (userToVet == null && vetToUser == null)
                {
                    throw new ArgumentNullException();
                }

                var distinctSender = userToVet.Distinct();
                var distinctReceiver = vetToUser.Distinct();
                var distinctAll = distinctSender.Concat(distinctReceiver);
                return distinctAll ;

            }

            public bool SaveChanges()
            {
                return (_context.SaveChanges()>=0);
            }

            public IEnumerable<Message> GetAllMessagesBySenderReceiverId(Guid senderId, Guid receiverId)
            {
                 var firstMessages = _context.Messages.ToList().Where(s=>
                     s.SenderId==senderId).Where(r=>r.ReceiverId == receiverId);
                 var secondMessages = _context.Messages.ToList().Where(s=>
                     s.SenderId==receiverId).Where(r=>r.ReceiverId == senderId);
                 var messages = firstMessages.Concat(secondMessages).OrderBy(m=>m.Date).ToList();
                return messages;
            }
        
        
        
        
        
    }
}