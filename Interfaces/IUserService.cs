using morse_service.DTO;

namespace morse_service.Interfaces.Services
{
    public interface IUserService
    {
        List<FriendDTO> GetFriends(int userId);
    }
}
