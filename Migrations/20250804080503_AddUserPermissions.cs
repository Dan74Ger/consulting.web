using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CanAccessGestioneClienti = table.Column<bool>(type: "bit", nullable: false),
                    CanAccessDatiUtenza = table.Column<bool>(type: "bit", nullable: false),
                    CanAccessReports = table.Column<bool>(type: "bit", nullable: false),
                    CanAccessViewBasicData = table.Column<bool>(type: "bit", nullable: false),
                    CanAccessAdvancedReports = table.Column<bool>(type: "bit", nullable: false),
                    CanAccessRestrictedArea = table.Column<bool>(type: "bit", nullable: false),
                    CanViewStatisticsInDashboard = table.Column<bool>(type: "bit", nullable: false),
                    CanViewPersonalReports = table.Column<bool>(type: "bit", nullable: false),
                    CanViewActivityHistory = table.Column<bool>(type: "bit", nullable: false),
                    CanViewAdminData = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPermissions");
        }
    }
}
