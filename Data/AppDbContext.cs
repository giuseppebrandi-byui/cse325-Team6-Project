using Microsoft.EntityFrameworkCore;
using MyMuscleCars.Models; 

namespace MyMuscleCars.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Make> Makes { get; set; }

  
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("account");
            modelBuilder.Entity<Inventory>().ToTable("inventory");
            modelBuilder.Entity<Make>().ToTable("make");

            //  Added a  table mapping for Car
            modelBuilder.Entity<Car>().ToTable("car");

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.MakeRef)
                .WithMany(m => m.Inventories)
                .HasForeignKey(i => i.MakeId);
        }
    }
}
