using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using TaskCraft.DataBase;
using System.Security.Claims;
using TaskCraft.Repositories;
using TaskCraft.DTOs;
using Infrastructure.Auth;



var builder = WebApplication.CreateBuilder();
builder.Services.AddAutoMapper(typeof(Program));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ChannelRepository>();
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true,
    });




var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();


#region UserEndPoints
app.MapPost("/users/register", async (UserRepository userRepository, RegisterUserDTO userDto) =>
{
    if (userDto == null || string.IsNullOrWhiteSpace(userDto.Login) || string.IsNullOrWhiteSpace(userDto.Password))
    {
        return Results.BadRequest("Invalid user data");
    }

    var existingUser = await userRepository.GetUserByLogin(userDto.Login);
    if (existingUser != null)
    {
        return Results.Conflict("User with this login already exists");
    }

    await userRepository.AddUser(userDto);
    return Results.Ok("User registered successfully");
});
app.MapGet("/users/{id}", async (UserRepository userRepository, Guid id) =>
{
    var user = await userRepository.GetUserById(id);
    if (user == null)
    {
        return Results.NotFound("User not found");
    }
    return Results.Ok(user);
});
app.MapGet("/users", async (UserRepository userRepository) =>
{
    var users = await userRepository.GetAllUsers();
    return Results.Ok(users);
});
app.MapPut("/users/put/{id}", [Authorize] async (HttpContext ctx, UserRepository userRepository, Guid id, UpdateUserDTO userDto) =>
{
    if (userDto == null || string.IsNullOrWhiteSpace(userDto.Login) || string.IsNullOrWhiteSpace(userDto.Password))
    {
        return Results.BadRequest("Invalid user data");
    }

    var existingUser = await userRepository.GetUserById(id);
    if (existingUser == null)
    {
        return Results.NotFound("User not found");
    }

    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    if (id != userId)
    {
        return Results.Forbid();
    }

    await userRepository.UpdateUser(id, userDto);
    return Results.Ok("User updated successfully");
});
app.MapDelete("/users/delete/{id}", [Authorize] async (HttpContext ctx, UserRepository userRepository, Guid id) =>
{
    var existingUser = await userRepository.GetUserById(id);
    if (existingUser == null)
    {
        return Results.NotFound("User not found");
    }

    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    if (id != userId)
    {
        return Results.Forbid();
    }

    await userRepository.DeleteUser(id);
    return Results.Ok("User deleted successfully");
});
app.MapPost("/users/login", async (UserRepository userRepository, LoginUserDTO loginDto) =>
{
    if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Login) || string.IsNullOrWhiteSpace(loginDto.Password))
    {
        return Results.BadRequest("Invalid login data");
    }

    var user = await userRepository.AuthenticateUser(loginDto.Login, loginDto.Password);
    if (user == null)
    {
        return Results.Unauthorized();
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Login),  
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())  
    };

    var token = AuthOptions.CreateToken(claims.ToDictionary(claim => claim.Type, claim => claim.Value));

    return Results.Ok(new { Token = token });
});
app.MapGet("/users/account", [Authorize] async (UserRepository userRepository, HttpContext ctx) =>
{
    string login = ctx.User.FindFirst(ClaimTypes.Name)?.Value;

    var user = await userRepository.GetUserByLogin(login);
    return Results.Ok(user);
});
#endregion

#region ChannelEndPoints
app.MapGet("/channels", [Authorize] async (ChannelRepository channelRepository, HttpContext ctx) =>
{
    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    var channels = await channelRepository.GetUserChannels(userId);
    return Results.Ok(channels);
});
app.MapGet("/channels/{id}", [Authorize] async (ChannelRepository channelRepository, HttpContext ctx, Guid id) =>
{
    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    
    // Проверяем, есть ли пользователь в проекте
    if (!await channelRepository.IsUserInChannel(id, userId))
    {
        return Results.Forbid();
    }

    var channel = await channelRepository.GetChannelById(id);
    if (channel == null)
    {
        return Results.NotFound("Channel not found");
    }
    return Results.Ok(channel);
});
app.MapPost("/channels/create", [Authorize] async (ChannelRepository channelRepository, HttpContext ctx, CreateChannelDTO channelDto) =>
{
    if (channelDto == null || string.IsNullOrWhiteSpace(channelDto.Name))
    {
        return Results.BadRequest("Invalid channel data");
    }

    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    await channelRepository.AddChannel(channelDto, userId, userId);
    return Results.Ok("Channel created successfully");
});
app.MapPut("/channels/put/{id}", [Authorize] async (ChannelRepository channelRepository, HttpContext ctx, Guid id, UpdateChannelDTO channelDto) =>
{
    if (channelDto == null || string.IsNullOrWhiteSpace(channelDto.Name))
    {
        return Results.BadRequest("Invalid channel data");
    }

    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    var channel = await channelRepository.GetChannelById(id);
    
    if (channel == null)
    {
        return Results.NotFound("Channel not found");
    }

    if (channel.OwnerId != userId)
    {
        return Results.Forbid();
    }

    var success = await channelRepository.UpdateChannel(id, channelDto);
    return success ? Results.Ok("Channel updated successfully") : Results.Problem("Failed to update channel");
});
app.MapDelete("/channels/delete/{id}", [Authorize] async (ChannelRepository channelRepository, HttpContext ctx, Guid id) =>
{
    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    var channel = await channelRepository.GetChannelById(id);
    
    if (channel == null)
    {
        return Results.NotFound("Channel not found");
    }

    if (channel.OwnerId != userId)
    {
        return Results.Forbid();
    }

    var success = await channelRepository.DeleteChannel(id);
    return success ? Results.Ok("Channel deleted successfully") : Results.Problem("Failed to delete channel");
});
app.MapPost("/channels/{channelId}/join", [Authorize] async (ChannelRepository channelRepository, HttpContext ctx, Guid channelId) =>
{
    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    var channel = await channelRepository.GetChannelById(channelId);
    if (channel == null)
    {
        return Results.NotFound("Channel not found");
    }


    if (channel.Users.Any(u => u.Id == userId))
    {
        return Results.Conflict("User is already in this channel");
    }

    await channelRepository.AddUserToChannel(channelId, userId);

    return Results.Ok("User successfully joined the channel");
});
#endregion

#region ChatEndPoints
app.MapPost("/channels/{channelId}/chats/create", [Authorize] async (ChatRepository chatRepository, ChannelRepository channelRepository, HttpContext ctx, Guid channelId, CreateChatDTO chatDto) =>
{
    if (chatDto == null || string.IsNullOrWhiteSpace(chatDto.Name))
    {
        return Results.BadRequest("Invalid chat data");
    }

    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    var channel = await channelRepository.GetChannelById(channelId);
    
    if (channel == null)
    {
        return Results.NotFound("Channel not found");
    }

    if (channel.OwnerId != userId)
    {
        return Results.Forbid();
    }

    var chatId = await chatRepository.AddChat(chatDto, userId, channelId);
    return Results.Ok(new { ChatId = chatId, Message = "Chat created successfully" });
});
app.MapGet("/channels/{channelId}/chats/{chatId}", [Authorize] async (
    ChatRepository chatRepository, 
    ChannelRepository channelRepository, 
    Guid channelId, 
    Guid chatId) =>
{
    var channel = await channelRepository.GetChannelById(channelId);
    if (channel == null)
    {
        return Results.NotFound("Channel not found");
    }

    var chat = await chatRepository.GetChatById(chatId);
    if (chat == null || chat.ChannelId != channelId)
    {
        return Results.NotFound("Chat not found");
    }

    return Results.Ok(chat);
});
app.MapPut("/channels/{channelId}/chats/put/{chatId}", [Authorize] async (
    ChatRepository chatRepository, 
    ChannelRepository channelRepository, 
    HttpContext ctx, 
    Guid channelId, 
    Guid chatId, 
    UpdateChatDTO chatDto) =>
{
    if (chatDto == null || string.IsNullOrWhiteSpace(chatDto.Name))
    {
        return Results.BadRequest("Invalid chat data");
    }

    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    var channel = await channelRepository.GetChannelById(channelId);
    if (channel == null)
    {
        return Results.NotFound("Channel not found");
    }

    if (channel.OwnerId != userId)
    {
        return Results.Forbid();
    }

    var chat = await chatRepository.GetChatById(chatId);
    if (chat == null || chat.ChannelId != channelId)
    {
        return Results.NotFound("Chat not found");
    }

    var success = await chatRepository.UpdateChat(chatId, chatDto);
    return success ? Results.Ok("Chat updated successfully") : Results.Problem("Failed to update chat");
});
app.MapDelete("/channels/{channelId}/chats/delete/{chatId}", [Authorize] async (
    ChatRepository chatRepository, 
    ChannelRepository channelRepository, 
    HttpContext ctx, 
    Guid channelId, 
    Guid chatId) =>
{
    var userId = Guid.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    var channel = await channelRepository.GetChannelById(channelId);
    if (channel == null)
    {
        return Results.NotFound("Channel not found");
    }

    if (channel.OwnerId != userId)
    {
        return Results.Forbid();
    }

    var chat = await chatRepository.GetChatById(chatId);
    if (chat == null || chat.ChannelId != channelId)
    {
        return Results.NotFound("Chat not found");
    }

    var success = await chatRepository.DeleteChat(chatId);
    return success ? Results.Ok("Chat deleted successfully") : Results.Problem("Failed to delete channel");
});
#endregion



app.Run();
