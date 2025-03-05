namespace TaskCraft.DTOs{
public class GetMessageDTO
{
    public Guid Id { get; set; }
    public string NickName { get; set; }
    public string ?Text { get; set; }
    public DateTime DateTime { get; set; }
}

public class CreateMessageDTO
{
    public string Text { get; set; }
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
}

public class UpdateMessageDTO{
    public string Text { get; set;}
}
}