using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitDatiUtenzaPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanAccessDatiUtenzaGenerale",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessDatiUtenzaRiservata",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAccessDatiUtenzaGenerale",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanAccessDatiUtenzaRiservata",
                table: "UserPermissions");
        }
    }
}
