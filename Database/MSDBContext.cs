using Microsoft.EntityFrameworkCore;
using morse_service.Database.Models;

namespace morse_service.Database
{
    public class MSDBContext : DbContext
    {
        public MSDBContext(DbContextOptions<MSDBContext> options) : base(options)
        { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserFriendsModel> UserFriends { get; set; }
    }
}
