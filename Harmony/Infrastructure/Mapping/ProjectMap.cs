using AutoMapper;
using TaskCraft.Entities;
using TaskCraft.DTOs;
using System.Linq;

public class ChannelProfile : Profile
{
    public ChannelProfile()
    {

        CreateMap<CreateChannelDTO, Channel>();


        CreateMap<UpdateChannelDTO, Channel>();

        CreateMap<Channel, GetChannelDTO>()
            .ForMember(dest => dest.OwnerNickName, opt => opt.MapFrom(src => src.Owner.NickName))
            .ForMember(dest => dest.ChatNames, opt => opt.MapFrom(src => src.Chats.Select(c => c.Name).ToList()))
            .ForMember(dest => dest.Chats, opt => opt.MapFrom(src => src.Chats))
            .ForMember(dest => dest.CallChats, opt => opt.MapFrom(src => src.CallChats));



        CreateMap<Channel, GetInListChannelDTO>();
    }
}