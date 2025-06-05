using AutoMapper;
using TaskCraft.Entities;
using TaskCraft.DTOs;

public class CallChatProfile : Profile
{
    public CallChatProfile()
    {
        // Маппинг из CreateCallChatDTO в CallChat
        CreateMap<CreateCallChatDTO, CallChat>();

        // Маппинг из UpdateCallChatDTO в CallChat
        CreateMap<UpdateCallChatDTO, CallChat>();

        // Маппинг из CallChat в GetCallChatDTO
        CreateMap<CallChat, GetCallChatDTO>()
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants.Select(p => p.UserId).ToList()));
    }
}
