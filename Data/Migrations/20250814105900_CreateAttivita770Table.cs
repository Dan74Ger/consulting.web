using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateAttivita770Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attivita_770",
                columns: table => new
                {
                    id_attivita_770 = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_anno = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_professionista = table.Column<int>(type: "int", nullable: true),
                    mod_770_lav_autonomo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    mod_770_ordinario = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    inserimento_dati_dr = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    dr_invio = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ricevuta = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    pec_invio_dr = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    mod_cu_fatte = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    cu_utili_presenti = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attivita_770", x => x.id_attivita_770);
                    table.ForeignKey(
                        name: "FK_attivita_770_anni_fiscali_id_anno",
                        column: x => x.id_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_770_clienti_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clienti",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_770_professionisti_id_professionista",
                        column: x => x.id_professionista,
                        principalTable: "professionisti",
                        principalColumn: "id_professionista");
                });

            migrationBuilder.CreateIndex(
                name: "IX_attivita_770_id_anno",
                table: "attivita_770",
                column: "id_anno");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_770_id_cliente",
                table: "attivita_770",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_770_id_professionista",
                table: "attivita_770",
                column: "id_professionista");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attivita_770");
        }
    }
}
