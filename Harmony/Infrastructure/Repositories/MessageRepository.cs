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
    public class MessageRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetMessageDTO?> GetMessageByIdAsync(Guid id)
        {
            var message = await _context.Messages
                .Include(m => m.Chat)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            return message != null ? _mapper.Map<GetMessageDTO>(message) : null;
        }

        public async Task AddMessageAsync(CreateMessageDTO messageDto)
        {
            var message = _mapper.Map<Message>(messageDto);
            message.Id = Guid.NewGuid();
            message.DateTime = DateTime.UtcNow;

            _context.Messages.Add(message);
            Console.WriteLine($"Message saved: {message.Text}, ChatId: {message.ChatId}, UserId: {message.UserId}");
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateMessageAsync(Guid messageId, UpdateMessageDTO messageDto)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
            if (message == null) return false;

            _mapper.Map(messageDto, message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
            if (message == null) return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<GetMessageDTO?> GetLastMessageByChatIdAsync(Guid chatId)
        {
            var message = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.DateTime)
                .FirstOrDefaultAsync();

            return message != null ? _mapper.Map<GetMessageDTO>(message) : null;
        }

public async Task<List<GetMessageDTO>> GetMessagesByChatIdAsync(Guid chatId)
{
    var messages = await _context.Messages
        .Include(m => m.User) 
        .Where(m => m.ChatId == chatId)
        .OrderBy(m => m.DateTime)
        .ToListAsync();



    return _mapper.Map<List<GetMessageDTO>>(messages);
}
    }
}