using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using morse_service.Database;
using morse_service.Database.Models;
using morse_service.DTO;
using morse_service.Hubs.DTO;
using morse_service.Interfaces.Services;

namespace morse_service.ServiceImplementations
{
    public class UserService : IUserService
    {
        private IDbContextFactory<MSDBContext> _contextFactory;

        public UserService(IDbContextFactory<MSDBContext> contextFactory) 
        { 
            _contextFactory = contextFactory;
        }

        public object[] FindUserByLoginExcludingRequestSource(int senderUserId, string login)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                List<UserDTO> result = new List<UserDTO>();

                var users = context.Users.Where(user => user.Login.Contains(login) && user.Id != senderUserId);
                foreach (var user in users)
                {
                    result.Add(new UserDTO
                    {
                        Id = user.Id,
                        Login = user.Login,
                        DisplayName = user.DisplayName,
                    });
                }
                return result.ToArray();
            }
        }

        public UserDTO? FindUserByLogin(string login)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var result = context.Users.FirstOrDefault(user => user.Login == login);
                if (result != null)
                {
                    return new UserDTO
                    { 
                        Id = result.Id,
                        Login = result.Login,
                        DisplayName = result.DisplayName
                    };
                }
                return null;
            }
        }
    }
}
