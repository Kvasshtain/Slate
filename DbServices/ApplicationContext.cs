using DrawingServices;
using Microsoft.EntityFrameworkCore;

namespace slate.DbServices
{
    public class ApplicationContext : DbContext
    {
        public DbSet<BlackboardObjectData> BlackboardObjectDatas { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) => Database.EnsureCreated();
    }
}
