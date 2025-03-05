using Microsoft.EntityFrameworkCore;
using TaskCraft.DataBase;
using TaskCraft.Repositories;



var builder = WebApplication.CreateBuilder();

builder.Services.AddAutoMapper(typeof(Program));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ChannelRepository>();
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<MessageRepository>();

var app = builder.Build();

app.Run();