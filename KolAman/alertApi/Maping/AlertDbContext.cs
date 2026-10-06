using alertApi.Models;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
namespace alertApi.Maping
{
    public class AlertDbContext : DbContext
    {
        public AlertDbContext(DbContextOptions<AlertDbContext> options) : base(options) { }

        public DbSet<Alert> Alerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Alert>(entity =>
            {



                entity.HasKey(a => a.AlertId);


                entity.Property(a => a.AlertId)
                      .ValueGeneratedNever();


                entity.Property(a => a.Title)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(a => a.Source)
                      .HasMaxLength(100);

                entity.Property(a => a.Priority)
                      .HasMaxLength(50);

                entity.Property(a => a.Classification)
                      .HasMaxLength(50);

                entity.Property(a => a.Status)
                      .HasMaxLength(50);




                entity.Property(a => a.Lat)
                      .HasPrecision(9, 6);

                entity.Property(a => a.Lon)
                      .HasPrecision(9, 6);
            });
        }
    }
}
