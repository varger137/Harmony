namespace TaskCraft.Entities{
public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string NickName { get; set; }
    public List<Channel> ?Channels { get; set; } = new List<Channel>();
    public List<Channel> OwnedChannels { get; set; } = new List<Channel>();
}
}