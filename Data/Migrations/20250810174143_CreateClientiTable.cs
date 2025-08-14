using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateClientiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Utilizzo SQL diretto per creare la tabella con i DEFAULT values corretti
            migrationBuilder.Sql(@"
                CREATE TABLE clienti (
                    id_cliente INT IDENTITY(1,1) PRIMARY KEY,
                    nome_cliente NVARCHAR(100) NOT NULL,
                    id_programma INT NULL,
                    id_professionista INT NULL,
                    mail_cliente NVARCHAR(100) NULL,
                    id_regime_contabile INT NULL,
                    id_tipologia_inps INT NULL,
                    contabilita BIT DEFAULT 0, -- interno/esterno
                    periodo_contabilita BIT DEFAULT 0, -- mensile/trimestrale
                    
                    -- Modelli esistenti
                    mod_730 BIT DEFAULT 0, -- ATTIVITA REDDITI
                    mod_740 BIT DEFAULT 0, -- ATTIVITA REDDITI
                    mod_750 BIT DEFAULT 0, -- ATTIVITA REDDITI
                    mod_760 BIT DEFAULT 0, -- ATTIVITA REDDITI
                    mod_770 BIT DEFAULT 0, -- ATTIVITA REDDITI
                    mod_cu BIT DEFAULT 0, -- ATTIVITA REDDITI
                    mod_enc BIT DEFAULT 0, -- ATTIVITA REDDITI
                    mod_irap BIT DEFAULT 0, -- ATTIVITA REDDITI
                    
                    driva BIT DEFAULT 0, -- ATTIVITA IVA
                    lipe BIT DEFAULT 0, -- ATTIVITA IVA
                    mod_tr_iva BIT DEFAULT 0, -- ATTIVITA IVA
                    
                    inail BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    cassetto_fiscale BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    fatturazione_elettronica_ts BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    conservazione BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    imu BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    reg_iva BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    reg_cespiti BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    inventari BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    libro_giornale BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    lettere_intento BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    mod_intrastat BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    firma_digitale BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    titolare_effettivo BIT DEFAULT 0, -- ATTIVITA CONTABILE
                    
                    -- CODICE ATECO - CAMPO MASTER
                    codice_ateco NVARCHAR(20) NULL,
                    
                    -- Campi di gestione stato
                    attivo BIT DEFAULT 1 NOT NULL,
                    data_attivazione DATETIME2 DEFAULT GETDATE() NOT NULL,
                    data_modifica DATETIME2 DEFAULT GETDATE() NOT NULL,
                    data_cessazione DATETIME2 NULL,
                    
                    -- Gestione riattivazione per anno fiscale
                    riattivato_per_anno INT NULL,
                    data_riattivazione DATETIME2 NULL,
                    
                    created_at DATETIME2 DEFAULT GETDATE(),
                    updated_at DATETIME2 DEFAULT GETDATE(),
                    
                    FOREIGN KEY (id_programma) REFERENCES tipo_programmi(id_programma),
                    FOREIGN KEY (id_professionista) REFERENCES professionisti(id_professionista),
                    FOREIGN KEY (id_regime_contabile) REFERENCES regimi_contabili(id_regime_contabile),
                    FOREIGN KEY (id_tipologia_inps) REFERENCES tipologie_inps(id_tipologia_inps),
                    FOREIGN KEY (riattivato_per_anno) REFERENCES anni_fiscali(id_anno)
                );
            ");

            // Creo gli indici
            migrationBuilder.CreateIndex(
                name: "IX_clienti_id_professionista",
                table: "clienti",
                column: "id_professionista");

            migrationBuilder.CreateIndex(
                name: "IX_clienti_id_programma",
                table: "clienti",
                column: "id_programma");

            migrationBuilder.CreateIndex(
                name: "IX_clienti_id_regime_contabile",
                table: "clienti",
                column: "id_regime_contabile");

            migrationBuilder.CreateIndex(
                name: "IX_clienti_id_tipologia_inps",
                table: "clienti",
                column: "id_tipologia_inps");

            migrationBuilder.CreateIndex(
                name: "IX_clienti_riattivato_per_anno",
                table: "clienti",
                column: "riattivato_per_anno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clienti");
        }
    }
}
