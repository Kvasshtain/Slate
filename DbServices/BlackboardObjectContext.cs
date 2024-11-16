using DrawingServices;
using Microsoft.EntityFrameworkCore;

namespace slate.DbServices
{
    public class BlackboardObjectContext : DbContext
    {
        public DbSet<BlackboardObjectData> BlackboardObjectDatas { get; set; } = null!;

        public BlackboardObjectContext(DbContextOptions<BlackboardObjectContext> options)
            : base(options) => Database.EnsureCreated();
    }
}
