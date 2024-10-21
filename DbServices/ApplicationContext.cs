using Microsoft.EntityFrameworkCore;
using slate.UsersServices;

namespace slate.DbServices
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new(1, "tom@gmail.com", "12345", "Вася Пупкин"),
                new(2, "bob@gmail.com", "55555", "Иванов Петя")
            );
        }
    }
}
