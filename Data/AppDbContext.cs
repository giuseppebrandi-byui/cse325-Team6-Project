using Microsoft.EntityFrameworkCore;
using MyMuscleCars.Models; // Make sure your models namespace is correct

namespace MyMuscleCars.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Make> Makes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("account");
            modelBuilder.Entity<Inventory>().ToTable("inventory");
            modelBuilder.Entity<Make>().ToTable("make");

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.MakeRef)
                .WithMany(m => m.Inventories)
                .HasForeignKey(i => i.MakeId);
        }
    }
}
