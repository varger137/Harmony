namespace TaskCraft.Entities
{
    public class CallChat
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public List<CallChatParticipant> Participants { get; set; } = new List<CallChatParticipant>();
    }
}
