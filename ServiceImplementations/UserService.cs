using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using morse_service.Database;
using morse_service.Database.Models;
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

        [Authorize]
        public List<FriendDTO> GetFriends(int userId)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                List<FriendDTO> friends = new List<FriendDTO>();

                // all ids that are not the given user
                var friendIds = context.UserFriends
                    .Where(relation => relation.UserIDA == userId || relation.UserIDB == userId)
                    .Select(relation => relation.UserIDA == userId ? relation.UserIDB : relation.UserIDA)
                    .ToList();

                foreach (var friendId in friendIds)
                {
                    var user = context.Users.FirstOrDefault(user => user.Id == friendId);
                    if (user != null)
                    {
                        friends.Add(new FriendDTO 
                        {
                            Id = user.Id,
                            Login = user.Login,
                            DisplayName = user.DisplayName,
                        });
                    }
                }
                return friends;
            }
        }
    }
}
