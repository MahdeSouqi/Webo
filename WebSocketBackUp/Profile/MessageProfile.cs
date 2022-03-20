using WebSocketServer.DTO;
using WebSocketServer.Model;

namespace WebSocketServer.Profile
{
    public class MessageProfile : AutoMapper.Profile
    {

        public MessageProfile()
        {
            CreateMap<MessageCreateDto, Message>();
            CreateMap<Message, MessageReadDto>();
        }
    }
}