using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateAttivitaIrapTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attivita_irap",
                columns: table => new
                {
                    id_attivita_irap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_anno = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_professionista = table.Column<int>(type: "int", nullable: true),
                    appuntamento_data_ora = table.Column<DateTime>(type: "datetime2", nullable: true),
                    codice_dr = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    raccolta_documenti = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    file_isa = table.Column<bool>(type: "bit", nullable: false),
                    ricevuta = table.Column<bool>(type: "bit", nullable: false),
                    cciaa = table.Column<bool>(type: "bit", nullable: false),
                    pec_invio_dr = table.Column<bool>(type: "bit", nullable: false),
                    dr_firmata = table.Column<bool>(type: "bit", nullable: false),
                    dr_inserita = table.Column<bool>(type: "bit", nullable: false),
                    dr_inserita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isa_dr_inseriti = table.Column<bool>(type: "bit", nullable: false),
                    isa_dr_inseriti_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    dr_controllata = table.Column<bool>(type: "bit", nullable: false),
                    dr_controllata_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    dr_spedita = table.Column<bool>(type: "bit", nullable: false),
                    dr_spedita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    numero_rate_f24_primo_acconto_saldo = table.Column<int>(type: "int", nullable: false),
                    f24_primo_acconto_saldo_consegnato = table.Column<bool>(type: "bit", nullable: false),
                    f24_secondo_acconto = table.Column<int>(type: "int", nullable: false),
                    f24_secondo_acconto_consegnato = table.Column<bool>(type: "bit", nullable: false),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attivita_irap", x => x.id_attivita_irap);
                    table.ForeignKey(
                        name: "FK_attivita_irap_anni_fiscali_id_anno",
                        column: x => x.id_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_irap_clienti_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clienti",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_irap_professionisti_id_professionista",
                        column: x => x.id_professionista,
                        principalTable: "professionisti",
                        principalColumn: "id_professionista");
                });

            migrationBuilder.CreateIndex(
                name: "IX_attivita_irap_id_anno",
                table: "attivita_irap",
                column: "id_anno");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_irap_id_cliente",
                table: "attivita_irap",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_irap_id_professionista",
                table: "attivita_irap",
                column: "id_professionista");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attivita_irap");
        }
    }
}
