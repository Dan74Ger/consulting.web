using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Migrations
{
    /// <inheritdoc />
    public partial class AddNomeBancaToBanche : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomeBanca",
                table: "Banche",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeBanca",
                table: "Banche");
        }
    }
}
