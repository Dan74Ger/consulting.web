using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateRegimiContabiliTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "regimi_contabili",
                columns: table => new
                {
                    id_regime_contabile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome_regime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    attivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    data_attivazione = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    data_modifica = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    data_cessazione = table.Column<DateTime>(type: "datetime2", nullable: true),
                    riattivato_per_anno = table.Column<int>(type: "int", nullable: true),
                    data_riattivazione = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regimi_contabili", x => x.id_regime_contabile);
                    table.ForeignKey(
                        name: "FK_regimi_contabili_anni_fiscali_riattivato_per_anno",
                        column: x => x.riattivato_per_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno");
                });

            migrationBuilder.CreateIndex(
                name: "IX_regimi_contabili_riattivato_per_anno",
                table: "regimi_contabili",
                column: "riattivato_per_anno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "regimi_contabili");
        }
    }
}
