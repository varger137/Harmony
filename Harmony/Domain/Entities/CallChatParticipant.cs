namespace TaskCraft.Entities
{
    public class CallChatParticipant
    {
        public Guid UserId { get; set; }
        public Guid CallChatId { get; set; }
        public CallChat CallChat { get; set; }
    }
}
