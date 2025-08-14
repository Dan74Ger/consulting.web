using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateProgrammiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tipo_programmi",
                columns: table => new
                {
                    id_programma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome_programma = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_tipo_programmi", x => x.id_programma);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tipo_programmi");
        }
    }
}
