using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskCraft.Entities;
using TaskCraft.DataBase;
using TaskCraft.DTOs;

public class CallChatRepository
{
    private readonly AppDbContext _context;

    public CallChatRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddCallChat(CreateCallChatDTO callChatDto, Guid ownerId, Guid channelId)
    {
        try
        {
            var callChat = new CallChat
            {
                Id = Guid.NewGuid(),
                Name = callChatDto.Name,
                OwnerId = ownerId,
                ChannelId = channelId,
                CreatedAt = DateTime.UtcNow,
                Participants = new List<CallChatParticipant>()
            };
            // ... rest of the method ...
            _context.CallChats.Add(callChat);
            await _context.SaveChangesAsync();
            return callChat.Id;
        }
        catch
        {
            return Guid.Empty;
        }
    }

    public async Task<GetCallChatDTO?> GetCallChatByIdAsync(Guid id)
    {
        var callChat = await _context.CallChats
            .Include(c => c.Channel)
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (callChat == null) return null;

        return new GetCallChatDTO
        {
            Id = callChat.Id,
            Name = callChat.Name,
            OwnerId = callChat.OwnerId,
            ChannelId = callChat.ChannelId,
            CreatedAt = callChat.CreatedAt,
            StartedAt = callChat.StartedAt,
            EndedAt = callChat.EndedAt,
            Participants = callChat.Participants.Select(p => p.UserId).ToList()
        };
    }

    public async Task<List<GetCallChatDTO>> GetCallChatsByChannelIdAsync(Guid channelId)
    {
        return await _context.CallChats
            .Where(c => c.ChannelId == channelId)
            .Include(c => c.Participants) // Add this line to include participants
            .Select(c => new GetCallChatDTO
            {
                Id = c.Id,
                Name = c.Name,
                OwnerId = c.OwnerId,
                ChannelId = c.ChannelId,
                CreatedAt = c.CreatedAt,
                StartedAt = c.StartedAt,
                EndedAt = c.EndedAt,
                Participants = c.Participants.Select(p => p.UserId).ToList()
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateCallChat(Guid id, UpdateCallChatDTO callChatDto)
    {
        var callChat = await _context.CallChats.FindAsync(id);
        if (callChat == null) return false;

        callChat.Name = callChatDto.Name;

        _context.CallChats.Update(callChat);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCallChat(Guid id)
    {
        var callChat = await _context.CallChats.FindAsync(id);
        if (callChat == null) return false;

        _context.CallChats.Remove(callChat);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> StartCall(Guid callChatId)
    {
        var callChat = await _context.CallChats.FindAsync(callChatId);
        if (callChat == null) return false;

        callChat.StartedAt = DateTime.UtcNow;
        _context.CallChats.Update(callChat);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EndCall(Guid callChatId)
    {
        var callChat = await _context.CallChats.FindAsync(callChatId);
        if (callChat == null) return false;

        callChat.EndedAt = DateTime.UtcNow;
        _context.CallChats.Update(callChat);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddParticipant(Guid callChatId, Guid userId)
    {
        var callChat = await _context.CallChats
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == callChatId);

        if (callChat == null) return false;

        if (!callChat.Participants.Any(p => p.UserId == userId))
        {
            callChat.Participants.Add(new CallChatParticipant
            {
                UserId = userId,
                CallChatId = callChatId
            });
            return await _context.SaveChangesAsync() > 0;
        }

        return true;
    }

    public async Task<bool> RemoveParticipant(Guid callChatId, Guid userId)
    {
        var callChat = await _context.CallChats
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == callChatId);

        if (callChat == null) return false;

        var participant = callChat.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant != null)
        {
            callChat.Participants.Remove(participant);
            return await _context.SaveChangesAsync() > 0;
        }

        return true;
    }
}
