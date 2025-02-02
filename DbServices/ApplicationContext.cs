using Microsoft.EntityFrameworkCore;
using slate.BlackboardsServices;
using slate.DrawingServices;
using slate.UsersServices;

namespace slate.DbServices
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Blackboard> Blackboards { get; set; } = null!;
        public DbSet<BlackboardObjectData> BlackboardObjectData { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) => Database.EnsureCreated();
    }
}
