using System;
using System.ComponentModel.DataAnnotations;

namespace WebSocketServer.DTO
{
    public class MessageCreateDto
    {   
        [Required]
        public Guid SenderId { get; set; }
        [Required]
        public Guid ReceiverId { get; set; }
        [Required]
        public string MessageText { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}