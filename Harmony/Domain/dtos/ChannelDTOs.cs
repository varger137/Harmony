using TaskCraft.Entities;

namespace TaskCraft.DTOs{
public class GetChannelDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<GetChatDTO>? Chats { get; set; }
    public List<GetCallChatDTO>? CallChats { get; set; }
    public List<string>? ChatNames { get; set; }
    public List<GetUserDTO> Users { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerNickName { get; set; }

}

public class CreateChannelDTO
{
    public string Name { get; set; }
    public string ?Description { get; set; }
    
}

public class GetInListChannelDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid Id { get; set; }
}

public class UpdateChannelDTO{
    public string Name { get; set;}
    public string Description { get; set; }
    
}
}