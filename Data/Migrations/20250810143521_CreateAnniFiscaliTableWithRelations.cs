using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateAnniFiscaliTableWithRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "anni_fiscali",
                columns: table => new
                {
                    id_anno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    anno = table.Column<int>(type: "int", nullable: false),
                    descrizione = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    attivo = table.Column<bool>(type: "bit", nullable: false),
                    anno_corrente = table.Column<bool>(type: "bit", nullable: false),
                    scadenza_730 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_740 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_750 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_760 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_ENC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_IRAP = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_770 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_CU = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_DIVA = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_Lipe_1t = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_Lipe_2t = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_Lipe_3t = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scadenza_Lipe_4t = table.Column<DateTime>(type: "datetime2", nullable: true),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anni_fiscali", x => x.id_anno);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tipo_programmi_riattivato_per_anno",
                table: "tipo_programmi",
                column: "riattivato_per_anno");

            // Crea l'indice per studios solo se non esiste già
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_studios_riattivato_per_anno' AND object_id = OBJECT_ID('studios'))
                BEGIN
                    CREATE INDEX [IX_studios_riattivato_per_anno] ON [studios] ([riattivato_per_anno])
                END
            ");

            migrationBuilder.CreateIndex(
                name: "IX_anni_fiscali_anno",
                table: "anni_fiscali",
                column: "anno",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_anni_fiscali_anno_corrente",
                table: "anni_fiscali",
                column: "anno_corrente",
                unique: true,
                filter: "[anno_corrente] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_studios_anni_fiscali_riattivato_per_anno",
                table: "studios",
                column: "riattivato_per_anno",
                principalTable: "anni_fiscali",
                principalColumn: "id_anno",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tipo_programmi_anni_fiscali_riattivato_per_anno",
                table: "tipo_programmi",
                column: "riattivato_per_anno",
                principalTable: "anni_fiscali",
                principalColumn: "id_anno",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_studios_anni_fiscali_riattivato_per_anno",
                table: "studios");

            migrationBuilder.DropForeignKey(
                name: "FK_tipo_programmi_anni_fiscali_riattivato_per_anno",
                table: "tipo_programmi");

            migrationBuilder.DropTable(
                name: "anni_fiscali");

            migrationBuilder.DropIndex(
                name: "IX_tipo_programmi_riattivato_per_anno",
                table: "tipo_programmi");

            migrationBuilder.DropIndex(
                name: "IX_studios_riattivato_per_anno",
                table: "studios");
        }
    }
}
