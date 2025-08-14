using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateProfessionistiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "professionisti",
                columns: table => new
                {
                    id_professionista = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    cognome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_professionisti", x => x.id_professionista);
                    table.ForeignKey(
                        name: "FK_professionisti_anni_fiscali_riattivato_per_anno",
                        column: x => x.riattivato_per_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno");
                });

            migrationBuilder.CreateIndex(
                name: "IX_professionisti_riattivato_per_anno",
                table: "professionisti",
                column: "riattivato_per_anno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "professionisti");
        }
    }
}
