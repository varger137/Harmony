namespace TaskCraft.DTOs{
public class CreateChatDTO
{
    public string Name { get; set; }
}

public class GetChatDTO
{
    
    public Guid Id { get; set; }
    public Guid ChannelId { get; set; }
    public string Name { get; set; }
    public List<GetMessageDTO> ?Messages { get; set; }
    public Guid ChatId { get; set; }
}

public class UpdateChatDTO{
    public string Name { get; set;}
}

}
