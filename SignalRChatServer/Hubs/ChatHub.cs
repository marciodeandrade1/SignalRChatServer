using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

namespace SignalRChatServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<ChatMessage> _messages;

        public ChatHub(IMongoDatabase database)
        {
            _messages = database.GetCollection<ChatMessage>("messages");
        }

        public async Task SendMessage(string user, string message)
        {
            var chatMessage = new ChatMessage
            {
                User = user,
                Message = message,
                Timestamp = DateTime.Now
            };

            // Persistir no MongoDB
            await _messages.InsertOneAsync(chatMessage);

            // Enviar para todos os clientes conectados
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }

    public class ChatMessage
    {
        public string User { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

