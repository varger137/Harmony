namespace TaskCraft.Entities{
public class Chat
{
    public Guid Id { get; set; }
    public string Name { get; set; }
     public Guid OwnerId { get; set; }
    public List<Message> ?Messages { get; set; } = new List<Message>();
    public Guid ChannelId { get; set; }
    public Channel Channel { get; set; }
    public DateTime CreatedAt { get; set; }

    }
}