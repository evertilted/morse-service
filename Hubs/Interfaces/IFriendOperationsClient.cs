namespace morse_service.Hubs.Interfaces
{
    public interface IFriendOperationsClient
    {
        void SendFriendRequest(int senderId, int recieverId);
        void RespondToFriendRequest(bool accepted);
    }
}
