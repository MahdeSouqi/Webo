using Microsoft.EntityFrameworkCore;
using WebSocketServer.Model;

namespace WebSocketServer.Data
{
    public class MessageContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public MessageContext(DbContextOptions<MessageContext> opt) : base(opt)
        {
            
        }
        
    }
}