using Microsoft.EntityFrameworkCore;
using slate.UsersServices;

namespace slate.DbServices
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public UserContext(DbContextOptions<UserContext> options)
            : base(options) => Database.EnsureCreated();
    }
}
