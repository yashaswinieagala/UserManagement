using Microsoft.EntityFrameworkCore;
using UserManagement.Shared.Models;

namespace UserManagement.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserDetail> Users => Set<UserDetail>();
    }

    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Users.Any()) return;

            db.Users.AddRange(
                new UserDetail { Id = 1, UserName = "alice_smith",   Email = "alice@example.com",   Age = 28, Department = "Engineering", IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-60) },
                new UserDetail { Id = 2, UserName = "bob_jones",     Email = "bob@example.com",     Age = 34, Department = "Design",      IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-45) },
                new UserDetail { Id = 3, UserName = "carol_white",   Email = "carol@example.com",   Age = 31, Department = "Product",     IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new UserDetail { Id = 4, UserName = "david_brown",   Email = "david@example.com",   Age = 25, Department = "QA",          IsActive = false, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new UserDetail { Id = 5, UserName = "eva_green",     Email = "eva@example.com",     Age = 40, Department = "HR",          IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new UserDetail { Id = 6, UserName = "frank_lee",     Email = "frank@example.com",   Age = 29, Department = "Engineering", IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new UserDetail { Id = 7, UserName = "grace_hall",    Email = "grace@example.com",   Age = 36, Department = "Marketing",   IsActive = false, CreatedAt = DateTime.UtcNow.AddDays(-5)  },
                new UserDetail { Id = 8, UserName = "henry_clark",   Email = "henry@example.com",   Age = 45, Department = "Finance",     IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-2)  }
            );
            db.SaveChanges();
        }
    }
}
