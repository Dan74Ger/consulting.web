using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTipologieInpsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crea la tabella tipologie_inps con DEFAULT values SQL Server
            migrationBuilder.Sql(@"
                CREATE TABLE tipologie_inps (
                    id_tipologia_inps INT IDENTITY(1,1) PRIMARY KEY,
                    tipologia NVARCHAR(100) NOT NULL,
                    
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
                    
                    FOREIGN KEY (riattivato_per_anno) REFERENCES anni_fiscali(id_anno)
                );
            ");

            // Crea l'indice per la foreign key
            migrationBuilder.Sql(@"
                CREATE INDEX IX_tipologie_inps_riattivato_per_anno 
                ON tipologie_inps (riattivato_per_anno);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tipologie_inps");
        }
    }
}
