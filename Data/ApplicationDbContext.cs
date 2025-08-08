using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Models;

namespace ConsultingGroup.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserPermissions> UserPermissions { get; set; } = default!;
        
        // Dati Utenza Tables
        public DbSet<Banche> Banche { get; set; } = default!;
        public DbSet<CarteCredito> CarteCredito { get; set; } = default!;
        public DbSet<Utenze> Utenze { get; set; } = default!;
        public DbSet<Cancelleria> Cancelleria { get; set; } = default!;
        public DbSet<Mail> Mail { get; set; } = default!;
        public DbSet<UtentiPC> UtentiPC { get; set; } = default!;
        public DbSet<AltriDati> AltriDati { get; set; } = default!;
        public DbSet<Entratel> Entratel { get; set; } = default!;
        public DbSet<UtentiTS> UtentiTS { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configurazioni aggiuntive per le entità se necessario
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            // Configurazione UserPermissions
            builder.Entity<UserPermissions>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurazioni Dati Utenza
            builder.Entity<Banche>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CarteCredito>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Utenze>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Cancelleria>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Mail>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UtentiPC>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AltriDati>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Entratel>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UtentiTS>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}