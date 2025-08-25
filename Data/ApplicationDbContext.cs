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
        public DbSet<AnnoFatturazione> AnniFatturazione { get; set; } = default!;
        public DbSet<ProformaGenerata> ProformeGenerate { get; set; } = default!;
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
        public DbSet<AttivitaLipe> AttivitaLipe { get; set; } = default!;
        public DbSet<AttivitaTriva> AttivitaTriva { get; set; } = default!;
        public DbSet<ContabilitaInternaTrimestrale> ContabilitaInternaTrimestrale { get; set; } = default!;
        public DbSet<ContabilitaInternaMensile> ContabilitaInternaMensile { get; set; } = default!;


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

            // Configurazioni precisione decimal per evitare avvisi EF
            
            // Cliente
            builder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.ImportoMandatoAnnuo).HasPrecision(18, 2);
                entity.Property(e => e.TassoIvaTrimestrale).HasPrecision(5, 4);
            });

            // ContabilitaInternaTrimestrale
            builder.Entity<ContabilitaInternaTrimestrale>(entity =>
            {
                // Primo Trimestre
                entity.Property(e => e.PrimoTrimestreLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.PrimoTrimestreCreditoAnnoPrecedente).HasPrecision(18, 2);
                entity.Property(e => e.PrimoTrimestreImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.PrimoTrimestreImportoDebito).HasPrecision(18, 2);
                entity.Property(e => e.PrimoTrimestreIvaVersare).HasPrecision(18, 2);
                
                // Secondo Trimestre
                entity.Property(e => e.SecondoTrimestreLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.SecondoTrimestreCreditoTrimestrePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.SecondoTrimestreImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.SecondoTrimestreImportoDebito).HasPrecision(18, 2);
                entity.Property(e => e.SecondoTrimestreIvaVersare).HasPrecision(18, 2);
                
                // Terzo Trimestre
                entity.Property(e => e.TerzoTrimestreLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.TerzoTrimestreCreditoTrimestrePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.TerzoTrimestreImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.TerzoTrimestreImportoDebito).HasPrecision(18, 2);
                entity.Property(e => e.TerzoTrimestreIvaVersare).HasPrecision(18, 2);
                
                // Quarto Trimestre
                entity.Property(e => e.QuartoTrimestreLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.QuartoTrimestreCreditoTrimestrePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.QuartoTrimestreImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.QuartoTrimestreImportoDebito).HasPrecision(18, 2);
                entity.Property(e => e.QuartoTrimestreIvaVersare).HasPrecision(18, 2);
                entity.Property(e => e.QuartoTrimestreAccontoIva).HasPrecision(18, 2);
            });

            // ContabilitaInternaMensile
            builder.Entity<ContabilitaInternaMensile>(entity =>
            {
                // Gennaio
                entity.Property(e => e.GennaioLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.GennaioCreditoAnnoPrecedente).HasPrecision(18, 2);
                entity.Property(e => e.GennaioImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.GennaioImportoDebito).HasPrecision(18, 2);
                
                // Febbraio
                entity.Property(e => e.FebbraioLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.FebbraioCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.FebbraioImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.FebbraioImportoDebito).HasPrecision(18, 2);
                
                // Marzo
                entity.Property(e => e.MarzoLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.MarzoCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.MarzoImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.MarzoImportoDebito).HasPrecision(18, 2);
                
                // Aprile
                entity.Property(e => e.AprileLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.AprileCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.AprileImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.AprileImportoDebito).HasPrecision(18, 2);
                
                // Maggio
                entity.Property(e => e.MaggioLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.MaggioCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.MaggioImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.MaggioImportoDebito).HasPrecision(18, 2);
                
                // Giugno
                entity.Property(e => e.GiugnoLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.GiugnoCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.GiugnoImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.GiugnoImportoDebito).HasPrecision(18, 2);
                
                // Luglio
                entity.Property(e => e.LuglioLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.LuglioCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.LuglioImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.LuglioImportoDebito).HasPrecision(18, 2);
                
                // Agosto
                entity.Property(e => e.AgostoLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.AgostoCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.AgostoImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.AgostoImportoDebito).HasPrecision(18, 2);
                
                // Settembre
                entity.Property(e => e.SettembreLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.SettembreCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.SettembreImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.SettembreImportoDebito).HasPrecision(18, 2);
                
                // Ottobre
                entity.Property(e => e.OttobreLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.OttobreCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.OttobreImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.OttobreImportoDebito).HasPrecision(18, 2);
                
                // Novembre
                entity.Property(e => e.NovembreLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.NovembreCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.NovembreImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.NovembreImportoDebito).HasPrecision(18, 2);
                
                // Dicembre
                entity.Property(e => e.DicembreLiqIvaImporto).HasPrecision(18, 2);
                entity.Property(e => e.DicembreCreditoMesePrecedente).HasPrecision(18, 2);
                entity.Property(e => e.DicembreImportoCredito).HasPrecision(18, 2);
                entity.Property(e => e.DicembreImportoDebito).HasPrecision(18, 2);
                entity.Property(e => e.DicembreAccontoIva).HasPrecision(18, 2);
            });

            // ProformaGenerata
            builder.Entity<ProformaGenerata>(entity =>
            {
                entity.Property(e => e.ImportoMandatoAnnuo).HasPrecision(18, 2);
                entity.Property(e => e.ImportoRata).HasPrecision(18, 2);
            });
        }
    }
}