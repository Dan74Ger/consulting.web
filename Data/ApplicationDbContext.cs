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
        
        // Anagrafiche Tables
        public DbSet<Studio> Studios { get; set; } = default!;
        public DbSet<Programma> Programmi { get; set; } = default!;
        public DbSet<AnnoFiscale> AnniFiscali { get; set; } = default!;
        public DbSet<Professionista> Professionisti { get; set; } = default!;
        public DbSet<RegimeContabile> RegimiContabili { get; set; } = default!;
        public DbSet<TipologiaInps> TipologieInps { get; set; } = default!;
        public DbSet<Cliente> Clienti { get; set; } = default!;
        
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

        // Attività Tables
        public DbSet<Attivita730> Attivita730 { get; set; } = default!;
        public DbSet<Attivita740> Attivita740 { get; set; } = default!;
        public DbSet<Attivita750> Attivita750 { get; set; } = default!;
        public DbSet<Attivita760> Attivita760 { get; set; } = default!;
        public DbSet<Attivita770> Attivita770 { get; set; } = default!;
        public DbSet<AttivitaIrap> AttivitaIrap { get; set; } = default!;
        public DbSet<AttivitaCu> AttivitaCu { get; set; } = default!;
        public DbSet<AttivitaEnc> AttivitaEnc { get; set; } = default!;
        public DbSet<AttivitaDriva> AttivitaDriva { get; set; } = default!;


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

            // Configurazione per Studios con trigger
            builder.Entity<Studio>(entity =>
            {
                // Disabilita l'uso delle clausole OUTPUT per questa entità
                // perché la tabella ha trigger che non sono compatibili
                entity.ToTable(tb => tb.HasTrigger("tr_studios_update_date"));
                
                // Relazione con AnnoFiscale
                entity.HasOne(e => e.AnnoFiscaleRiattivazione)
                      .WithMany(a => a.StudiosRiattivati)
                      .HasForeignKey(e => e.RiattivatoPerAnno)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configurazione per Programmi con trigger
            builder.Entity<Programma>(entity =>
            {
                // Disabilita l'uso delle clausole OUTPUT per questa entità
                // perché la tabella ha trigger che non sono compatibili
                entity.ToTable(tb => tb.HasTrigger("tr_tipo_programmi_update_date"));
                
                // Relazione con AnnoFiscale
                entity.HasOne(e => e.AnnoFiscaleRiattivazione)
                      .WithMany(a => a.ProgrammiRiattivati)
                      .HasForeignKey(e => e.RiattivatoPerAnno)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configurazione per AnnoFiscale
            builder.Entity<AnnoFiscale>(entity =>
            {
                // Solo un anno può essere corrente alla volta
                entity.HasIndex(e => e.AnnoCorrente)
                      .HasFilter("[anno_corrente] = 1")
                      .IsUnique();
                      
                // L'anno deve essere unico
                entity.HasIndex(e => e.Anno)
                      .IsUnique();
            });
        }
    }
}