using System;
using System.Collections.Generic;

public class CreateCallChatDTO
{
    public string Name { get; set; }
    public bool IsVideoEnabled { get; set; }
}

public class UpdateCallChatDTO
{
    public string Name { get; set; }
    public bool IsVideoEnabled { get; set; }
}

public class GetCallChatDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ChannelId { get; set; }
    public bool IsVideoEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public List<Guid> Participants { get; set; } = new List<Guid>();
}
