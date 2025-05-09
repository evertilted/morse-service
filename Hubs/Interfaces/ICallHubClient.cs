using morse_service.DTO;

namespace morse_service.Hubs.Interfaces
{
    public interface ICallHubClient
    {
        Task ReceiveUserSearchResult(object[] users);
    }
}
