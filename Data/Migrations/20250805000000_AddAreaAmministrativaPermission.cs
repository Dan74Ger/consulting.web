using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaAmministrativaPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanAccessAreaAmministrativa",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAccessAreaAmministrativa",
                table: "UserPermissions");
        }
    }
}
