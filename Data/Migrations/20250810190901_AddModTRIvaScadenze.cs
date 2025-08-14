using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModTRIvaScadenze : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "scadenza_Mod_TR_Iva_1t",
                table: "anni_fiscali",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "scadenza_Mod_TR_Iva_2t",
                table: "anni_fiscali",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "scadenza_Mod_TR_Iva_3t",
                table: "anni_fiscali",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "scadenza_Mod_TR_Iva_4t",
                table: "anni_fiscali",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "scadenza_Mod_TR_Iva_1t",
                table: "anni_fiscali");

            migrationBuilder.DropColumn(
                name: "scadenza_Mod_TR_Iva_2t",
                table: "anni_fiscali");

            migrationBuilder.DropColumn(
                name: "scadenza_Mod_TR_Iva_3t",
                table: "anni_fiscali");

            migrationBuilder.DropColumn(
                name: "scadenza_Mod_TR_Iva_4t",
                table: "anni_fiscali");
        }
    }
}
