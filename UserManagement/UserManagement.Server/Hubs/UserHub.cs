using Microsoft.AspNetCore.SignalR;
using UserManagement.Shared.Models;

namespace UserManagement.Server.Hubs
{
    public class UserHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveNotification", new UserNotification
            {
                Action = "Connected",
                Message = "You are connected to live updates.",
                Timestamp = DateTime.UtcNow
            });
            await base.OnConnectedAsync();
        }
    }
}
