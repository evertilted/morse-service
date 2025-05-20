using morse_service.DTO;

namespace morse_service.Interfaces.Services
{
    public interface IUserService
    {
        object[] FindUserByLoginExcludingRequestSource(int senderUserId, string login);

        UserDTO? FindUserByLogin(string login);
    }
}
