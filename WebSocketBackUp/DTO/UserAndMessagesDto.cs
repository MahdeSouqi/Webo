using System;
using System.Collections.Generic;

namespace WebSocketServer.DTO
{
    public class UserAndMessagesDto
    {
        public Guid UserId { get; set; }
        public IEnumerable<MessageReadDto> Message { get; set; }
    }
}