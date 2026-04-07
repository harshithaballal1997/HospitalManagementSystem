using Hospital.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Hospital.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IAIChatService _aiChatService;

        public ChatHub(IAIChatService aiChatService)
        {
            _aiChatService = aiChatService;
        }

        public async Task SendMessage(string message)
        {
            // Send the user's message back to their own screen
            await Clients.Caller.SendAsync("ReceiveMessage", "You", message);

            // Process query through the AI Sandbox, explicitly passing Context.User
            string botResponse = await _aiChatService.ProcessQueryAsync(message, Context.User);

            // Send the bot's response
            await Clients.Caller.SendAsync("ReceiveMessage", "Hospital AI", botResponse);
        }
    }
}
