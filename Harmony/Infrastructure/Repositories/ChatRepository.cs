using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskCraft.Entities;
using TaskCraft.DataBase;
using TaskCraft.DTOs;
using AutoMapper;

namespace TaskCraft.Repositories
{
    public class ChatRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ChatRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetChatDTO?> GetChatById(Guid id)
        {
            var chat = await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
            return chat != null ? _mapper.Map<GetChatDTO>(chat) : null;
        }
        public async Task<Guid> AddChat(CreateChatDTO chatDto, Guid ownerId, Guid channelId)
        {
            var chat = _mapper.Map<Chat>(chatDto);
            chat.Id = Guid.NewGuid();
            chat.OwnerId = ownerId;
            chat.ChannelId = channelId;
            chat.CreatedAt = DateTime.UtcNow;

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return chat.Id;
        }
        public async Task<bool> UpdateChat(Guid id, UpdateChatDTO chatDto)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == id);
            if (chat == null) return false;

            _mapper.Map(chatDto, chat);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<bool> DeleteChat(Guid id)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == id);
            if (chat == null) return false;

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}