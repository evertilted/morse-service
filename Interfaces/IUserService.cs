using morse_service.DTO;

namespace morse_service.Interfaces.Services
{
    public interface IUserService
    {
        object[] FindUserByLogin(int senderUserId, string login);
    }
}
