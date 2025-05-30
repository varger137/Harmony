using System;
using System.Collections.Generic;

public class CreateCallChatDTO
{
    public string Name { get; set; }
}

public class UpdateCallChatDTO
{
    public string Name { get; set; }
}

public class GetCallChatDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ChannelId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public List<Guid> Participants { get; set; } = new List<Guid>();
}

public class GetCallChatParticipantDTO
{
    public Guid UserId { get; set; }
    public string NickName { get; set; }
}
