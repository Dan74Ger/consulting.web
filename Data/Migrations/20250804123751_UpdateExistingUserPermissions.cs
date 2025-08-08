using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingUserPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAccessAdvancedReports",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanAccessDatiUtenza",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanAccessReports",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanAccessRestrictedArea",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanAccessViewBasicData",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanViewActivityHistory",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanViewAdminData",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanViewPersonalReports",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanViewStatisticsInDashboard",
                table: "UserPermissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanAccessAdvancedReports",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessDatiUtenza",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessReports",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessRestrictedArea",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessViewBasicData",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewActivityHistory",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewAdminData",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewPersonalReports",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewStatisticsInDashboard",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
