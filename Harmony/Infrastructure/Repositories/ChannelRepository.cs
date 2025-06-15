using TaskCraft.DataBase;
using Microsoft.EntityFrameworkCore;
using TaskCraft.Entities;
using TaskCraft.DTOs;
using AutoMapper;
using System;


namespace TaskCraft.Repositories
{
    public class ChannelRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ChannelRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetChannelDTO?> GetChannelById(Guid id)
        {
            var channel = await _context.Channels
                .Include(p => p.Chats)
                .Include(p => p.CallChats)
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == id);

            return channel != null ? _mapper.Map<GetChannelDTO>(channel) : null;
        }

        public async Task<List<GetChannelDTO>> GetAllChannels()
        {
            var channels = await _context.Channels
                .Include(p => p.Owner)
                .Include(p => p.Chats)
                .Include(p => p.CallChats)
                .Include(p => p.Users)
                .ToListAsync();

            return _mapper.Map<List<GetChannelDTO>>(channels);
        }

        public async Task<List<GetChannelDTO>> GetUserChannels(Guid userId)
        {
            var channels = await _context.Channels
                .Include(p => p.Owner)
                .Include(p => p.Chats)
                .Include(p => p.CallChats)
                .Include(p => p.Users)
                .Where(p => p.Users.Any(u => u.Id == userId))
                .ToListAsync();

            return _mapper.Map<List<GetChannelDTO>>(channels);
        }

        public async Task AddChannel(CreateChannelDTO channelDto, Guid ownerId, Guid userId)
        {
            var channel = _mapper.Map<Channel>(channelDto);
            var user = await _context.Users
            .Include(u => u.Channels)
            .FirstOrDefaultAsync(u => u.Id == userId);

            channel.Id = Guid.NewGuid();
            channel.OwnerId = ownerId;
            channel.CreatedAt = DateTime.UtcNow;

            var owner = await _context.Users.FindAsync(ownerId);
            if (owner != null)
            {
                owner.Channels.Add(channel);
                channel.Users.Add(user);
            }




            _context.Channels.Add(channel);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateChannel(Guid channelId, UpdateChannelDTO channelDto)
        {
            var channel = await _context.Channels
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == channelId);
            if (channel == null) return false;

            _mapper.Map(channelDto, channel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddUserToChannel(Guid channelId, Guid userId)
        {
            var channel = await _context.Channels
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == channelId);

            var user = await _context.Users
                .Include(u => u.Channels)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (channel == null || user == null)
            {
                return false;
            }


            if (!channel.Users.Any(u => u.Id == userId))
            {

                channel.Users.Add(user);
            }



            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserInChannel(Guid channelId, Guid userId)
        {

            return await _context.Channels
                .Where(p => p.Id == channelId)
                .AnyAsync(p => p.Users.Any(u => u.Id == userId));
        }

        public async Task<bool> DeleteChannel(Guid id)
        {
            var channel = await _context.Channels.FirstOrDefaultAsync(p => p.Id == id);
            if (channel == null)
            {
                return false;
            }

            _context.Channels.Remove(channel);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Channel?> GetChannelByChatId(Guid chatId)
        {
            return await _context.Chats
                .Where(c => c.Id == chatId)
                .Include(c => c.Channel)
                .Select(c => c.Channel)
                .FirstOrDefaultAsync();
        }
        public async Task<List<User>> GetChannelUsersAsync(Guid channelId)
{
    var channel = await _context.Channels
        .Include(c => c.Users)
        .FirstOrDefaultAsync(c => c.Id == channelId);

    return channel?.Users.ToList() ?? new List<User>();
}
        public async Task<bool> RemoveUserFromChannel(Guid channelId, Guid userId)
        {
            var channel = await _context.Channels
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == channelId);

            var user = await _context.Users
                .Include(u => u.Channels)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (channel == null || user == null)
            {
                return false;
            }

            // Удаляем пользователя из канала
            channel.Users.Remove(user);
            
            // Удаляем канал из списка каналов пользователя
            user.Channels.Remove(channel);

            await _context.SaveChangesAsync();
            return true;
        }
    }
    
}