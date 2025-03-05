using AutoMapper;
using TaskCraft.Entities;
using TaskCraft.DTOs;

public class MessageProfile : Profile
{
    public MessageProfile()
    {

        CreateMap<CreateMessageDTO, Message>();


        CreateMap<UpdateMessageDTO, Message>();


        CreateMap<Message, GetMessageDTO>()
            .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.User.NickName)); 
    }
}