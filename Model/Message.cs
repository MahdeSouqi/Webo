using System;
using System.ComponentModel.DataAnnotations;

namespace WebSocketServer.Model
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid SenderId { get; set; }
        [Required]
        public Guid ReceiverId { get; set; }
        [Required]
        public string MessageText { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool HaveSeen { get; set; }
    }
}