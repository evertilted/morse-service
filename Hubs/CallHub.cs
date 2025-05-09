using Microsoft.AspNetCore.SignalR;
using morse_service.Hubs.Interfaces;
using morse_service.Interfaces.Services;

namespace morse_service.Hubs
{
    public class CallHub : Hub<ICallHubClient>
    {
        private readonly IUserService _userService;

        public CallHub(IUserService userService)
        {
            _userService = userService;
        }

        public async Task FindUserByLogin(int senderUserId, string login)
        {
            await Clients.Caller.ReceiveUserSearchResult(_userService.FindUserByLogin(senderUserId, login));
        } 
    }
}
