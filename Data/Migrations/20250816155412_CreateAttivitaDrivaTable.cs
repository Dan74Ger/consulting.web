using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateAttivitaDrivaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attivita_cu",
                columns: table => new
                {
                    id_attivita_cu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_anno = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_professionista = table.Column<int>(type: "int", nullable: true),
                    cu_lav_autonomo = table.Column<bool>(type: "bit", nullable: false),
                    cu_utili = table.Column<bool>(type: "bit", nullable: false),
                    invio_cu = table.Column<bool>(type: "bit", nullable: false),
                    num_cu = table.Column<int>(type: "int", nullable: true),
                    ricevuta_cu = table.Column<bool>(type: "bit", nullable: false),
                    invio_cliente_mail = table.Column<bool>(type: "bit", nullable: false),
                    invio_cliente_mail_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attivita_cu", x => x.id_attivita_cu);
                    table.ForeignKey(
                        name: "FK_attivita_cu_anni_fiscali_id_anno",
                        column: x => x.id_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_cu_clienti_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clienti",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_cu_professionisti_id_professionista",
                        column: x => x.id_professionista,
                        principalTable: "professionisti",
                        principalColumn: "id_professionista");
                });

            migrationBuilder.CreateTable(
                name: "attivita_driva",
                columns: table => new
                {
                    id_attivita_driva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_anno = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_professionista = table.Column<int>(type: "int", nullable: true),
                    codice_dr_iva = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    appuntamento_data_ora = table.Column<DateTime>(type: "datetime2", nullable: true),
                    acconto_iva_tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    acconto_iva_credito_debito = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    importo_acconto_iva = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f24_acconto_iva_stato = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    raccolta_documenti = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    driva_inserita = table.Column<bool>(type: "bit", nullable: false),
                    driva_inserita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    driva_controllata = table.Column<bool>(type: "bit", nullable: false),
                    driva_controllata_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    driva_credito_debito = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    importo_dr_iva = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f24_driva_consegnato = table.Column<bool>(type: "bit", nullable: false),
                    f24_driva_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    dr_visto = table.Column<bool>(type: "bit", nullable: false),
                    ricevuta_driva = table.Column<bool>(type: "bit", nullable: false),
                    driva_spedita = table.Column<bool>(type: "bit", nullable: false),
                    driva_spedita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    tcg_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attivita_driva", x => x.id_attivita_driva);
                    table.ForeignKey(
                        name: "FK_attivita_driva_anni_fiscali_id_anno",
                        column: x => x.id_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_driva_clienti_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clienti",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_driva_professionisti_id_professionista",
                        column: x => x.id_professionista,
                        principalTable: "professionisti",
                        principalColumn: "id_professionista");
                });

            migrationBuilder.CreateIndex(
                name: "IX_attivita_cu_id_anno",
                table: "attivita_cu",
                column: "id_anno");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_cu_id_cliente",
                table: "attivita_cu",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_cu_id_professionista",
                table: "attivita_cu",
                column: "id_professionista");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_driva_id_anno",
                table: "attivita_driva",
                column: "id_anno");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_driva_id_cliente",
                table: "attivita_driva",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_driva_id_professionista",
                table: "attivita_driva",
                column: "id_professionista");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attivita_cu");

            migrationBuilder.DropTable(
                name: "attivita_driva");
        }
    }
}
