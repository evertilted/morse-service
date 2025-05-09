using Microsoft.AspNetCore.SignalR;
using morse_service.Hubs.DTO;

namespace morse_service.Hubs
{
    public class FriendHub : Hub
    {
        public async Task SendRequest(int senderUserId, int recieverUserId)
        {
            await Clients.All.SendAsync("RecieveFriendRequest", senderUserId, "request text");
        } 
    }
}
