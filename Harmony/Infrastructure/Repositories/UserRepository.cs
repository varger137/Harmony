using TaskCraft.DataBase;
using TaskCraft.Entities;
using TaskCraft.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BCrypt.Net;

namespace TaskCraft.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;  


        public UserRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<GetUserDTO> GetUserById(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Channels)  
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            var userDto = _mapper.Map<GetUserDTO>(user);
            userDto.Channels = _mapper.Map<List<GetInListChannelDTO>>(user.Channels);

            return userDto;
        }

        public async Task<User> AuthenticateUser(string login, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);


            if (user != null && BCrypt.Verify(password, user.Password))
            {
                return user; 
            }

            return null; 
        }
                public async Task<GetUserDTO> GetUserByLogin(string Login)
        {
            var user = await _context.Users
                .Include(u => u.Channels)  
                .FirstOrDefaultAsync(u => u.Login == Login);

            return user != null ? _mapper.Map<GetUserDTO>(user) : null;
        }


        public async Task AddUser(RegisterUserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.Password = BCrypt.HashPassword(user.Password);  // Хешируем пароль

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateUser(Guid id, UpdateUserDTO userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _mapper.Map(userDto, user);
                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    user.Password = BCrypt.HashPassword(userDto.Password);  // Хешируем новый пароль
                }
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<GetUserDTO>> GetAllUsers()
        {
            var users = await _context.Users.Include(u => u.Channels).ToListAsync();
            return _mapper.Map<List<GetUserDTO>>(users);
        }
    }
}