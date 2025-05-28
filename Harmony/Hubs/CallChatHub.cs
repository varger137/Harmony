using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Harmony.Hubs
{
    public class CallChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> CallChatParticipants = new();

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var callChatId = httpContext?.Request.Query["callChatId"].ToString();
            if (string.IsNullOrEmpty(callChatId))
                return;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"CallChat_{callChatId}");

            var participants = CallChatParticipants.GetOrAdd(callChatId, _ => new HashSet<string>());

            lock (participants)
            {
                participants.Add(Context.ConnectionId);
            }

            // Отправить новому участнику список уже подключённых
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveSignal", 
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    type = "participantList",
                    participants = participants.Where(id => id != Context.ConnectionId).ToList()
                }));

            // Уведомить остальных о новом участнике
            await Clients.GroupExcept($"CallChat_{callChatId}", Context.ConnectionId)
                         .SendAsync("ReceiveSignal",
                            System.Text.Json.JsonSerializer.Serialize(new
                            {
                                type = "newParticipant",
                                senderId = Context.ConnectionId
                            }));

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var callChatId = httpContext?.Request.Query["callChatId"].ToString();
            if (!string.IsNullOrEmpty(callChatId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"CallChat_{callChatId}");

                if (CallChatParticipants.TryGetValue(callChatId, out var participants))
                {
                    lock (participants)
                    {
                        participants.Remove(Context.ConnectionId);
                    }
                }

                await Clients.Group($"CallChat_{callChatId}")
                             .SendAsync("ReceiveSignal",
                                System.Text.Json.JsonSerializer.Serialize(new
                                {
                                    type = "left",
                                    senderId = Context.ConnectionId
                                }));
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendSignal(string signalData)
        {
            var httpContext = Context.GetHttpContext();
            var callChatId = httpContext?.Request.Query["callChatId"].ToString();
            if (string.IsNullOrEmpty(callChatId))
                return;

            // Пробуем определить, кому отправить сигнал
            var parsed = System.Text.Json.JsonDocument.Parse(signalData);
            if (parsed.RootElement.TryGetProperty("receiverId", out var receiverIdElement))
            {
                var receiverId = receiverIdElement.GetString();
                if (!string.IsNullOrEmpty(receiverId))
                {
                    // Отправляем конкретному клиенту
                    await Clients.Client(receiverId)
                        .SendAsync("ReceiveSignal", signalData);
                    return;
                }
            }

            // По умолчанию рассылаем всем, кроме отправителя
            await Clients.GroupExcept($"CallChat_{callChatId}", Context.ConnectionId)
                         .SendAsync("ReceiveSignal", signalData);
        }
    }
}
