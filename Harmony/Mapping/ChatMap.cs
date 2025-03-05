using AutoMapper;
using TaskCraft.Entities;
using TaskCraft.DTOs;

namespace TaskCraft.Maps
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {

            CreateMap<CreateChatDTO, Chat>();


            CreateMap<UpdateChatDTO, Chat>();


            CreateMap<Chat, GetChatDTO>()
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Id));
        }
    }
}