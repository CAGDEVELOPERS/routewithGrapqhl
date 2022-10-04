
using API.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Clients> Clients { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(GetConnectionString());
        }

        private static string GetConnectionString()
        {
            const string databaseName = "rutasProd";
            const string databaseUser = "rutas";
            const string databasePass = "22E7i@Rop!6VLD*!HAka";

            return $"Server=54.177.203.25;" +
                   $"database={databaseName};" +
                   $"uid={databaseUser};" +
                   $"pwd={databasePass};" +
                   $"pooling=true;";
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<IdentityUser>(entity =>
            //{
            //    entity.ToTable(name: "User");
            //});

            //builder.Entity<IdentityUser>()
            //    .ToTable("Users", "dbo");//.Property(p => p.Id).HasColumnName("User_Id");
            //builder.Entity<User>()
            //    .ToTable("Users", "dbo");//.Property(p => p.Id).HasColumnName("User_Id");

            //builder.Entity<IdentityUser>(entity =>
            //{
            //    entity.ToTable(name: "Users");
            //});
            base.OnModelCreating(builder);
            builder.Entity<User>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<User>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
            builder.Entity<User>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

            builder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));

            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));

            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));

            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));
        }
    }
    public class User : IdentityUser
    {
        public decimal Latitud { get; set; }

        public decimal Longitud { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public byte[] img { get; set; }
    }
}
