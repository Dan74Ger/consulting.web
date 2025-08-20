using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingGroup.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContabilitaInternaTrimestrale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cap",
                table: "clienti",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cf_cliente",
                table: "clienti",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cf_legale_rappresentante",
                table: "clienti",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citta",
                table: "clienti",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_mandato",
                table: "clienti",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "importo_mandato_annuo",
                table: "clienti",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "indirizzo",
                table: "clienti",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "legale_rappresentante",
                table: "clienti",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "piva_cliente",
                table: "clienti",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "proforma_tipo",
                table: "clienti",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "provincia",
                table: "clienti",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "importo_dr_iva",
                table: "attivita_driva",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "importo_acconto_iva",
                table: "attivita_driva",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "anni_fatturazione",
                columns: table => new
                {
                    id_anno_fatturazione = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    anno = table.Column<int>(type: "int", nullable: false),
                    descrizione = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    attivo = table.Column<bool>(type: "bit", nullable: false),
                    anno_corrente = table.Column<bool>(type: "bit", nullable: false),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anni_fatturazione", x => x.id_anno_fatturazione);
                });

            migrationBuilder.CreateTable(
                name: "attivita_lipe",
                columns: table => new
                {
                    id_attivita_lipe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_anno = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_professionista = table.Column<int>(type: "int", nullable: true),
                    t1_raccolta_documenti = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    t1_lipe_inserita = table.Column<bool>(type: "bit", nullable: false),
                    t1_lipe_inserita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t1_lipe_controllata = table.Column<bool>(type: "bit", nullable: false),
                    t1_lipe_controllata_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t1_lipe_spedita = table.Column<bool>(type: "bit", nullable: false),
                    t1_lipe_spedita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t1_created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    t1_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    t2_raccolta_documenti = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    t2_lipe_inserita = table.Column<bool>(type: "bit", nullable: false),
                    t2_lipe_inserita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t2_lipe_controllata = table.Column<bool>(type: "bit", nullable: false),
                    t2_lipe_controllata_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t2_lipe_spedita = table.Column<bool>(type: "bit", nullable: false),
                    t2_lipe_spedita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t2_created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    t2_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    t3_raccolta_documenti = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    t3_lipe_inserita = table.Column<bool>(type: "bit", nullable: false),
                    t3_lipe_inserita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t3_lipe_controllata = table.Column<bool>(type: "bit", nullable: false),
                    t3_lipe_controllata_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t3_lipe_spedita = table.Column<bool>(type: "bit", nullable: false),
                    t3_lipe_spedita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t3_created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    t3_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    t4_raccolta_documenti = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    t4_lipe_inserita = table.Column<bool>(type: "bit", nullable: false),
                    t4_lipe_inserita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t4_lipe_controllata = table.Column<bool>(type: "bit", nullable: false),
                    t4_lipe_controllata_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t4_lipe_spedita = table.Column<bool>(type: "bit", nullable: false),
                    t4_lipe_spedita_data = table.Column<DateTime>(type: "datetime2", nullable: true),
                    t4_created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    t4_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attivita_lipe", x => x.id_attivita_lipe);
                    table.ForeignKey(
                        name: "FK_attivita_lipe_anni_fiscali_id_anno",
                        column: x => x.id_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_lipe_clienti_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clienti",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_lipe_professionisti_id_professionista",
                        column: x => x.id_professionista,
                        principalTable: "professionisti",
                        principalColumn: "id_professionista");
                });

            migrationBuilder.CreateTable(
                name: "attivita_mod_tr_iva",
                columns: table => new
                {
                    id_mod_tr_iva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_anno = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    primo_trimestre = table.Column<bool>(type: "bit", nullable: false),
                    primo_trimestre_compilato = table.Column<bool>(type: "bit", nullable: false),
                    primo_trimestre_spedito = table.Column<bool>(type: "bit", nullable: false),
                    primo_trimestre_ricevuta = table.Column<bool>(type: "bit", nullable: false),
                    primo_trimestre_mail_cliente = table.Column<bool>(type: "bit", nullable: false),
                    secondo_trimestre = table.Column<bool>(type: "bit", nullable: false),
                    secondo_trimestre_compilato = table.Column<bool>(type: "bit", nullable: false),
                    secondo_trimestre_spedito = table.Column<bool>(type: "bit", nullable: false),
                    secondo_trimestre_ricevuta = table.Column<bool>(type: "bit", nullable: false),
                    secondo_trimestre_mail_cliente = table.Column<bool>(type: "bit", nullable: false),
                    terzo_trimestre = table.Column<bool>(type: "bit", nullable: false),
                    terzo_trimestre_compilato = table.Column<bool>(type: "bit", nullable: false),
                    terzo_trimestre_spedito = table.Column<bool>(type: "bit", nullable: false),
                    terzo_trimestre_ricevuta = table.Column<bool>(type: "bit", nullable: false),
                    terzo_trimestre_mail_cliente = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attivita_mod_tr_iva", x => x.id_mod_tr_iva);
                    table.ForeignKey(
                        name: "FK_attivita_mod_tr_iva_anni_fiscali_id_anno",
                        column: x => x.id_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attivita_mod_tr_iva_clienti_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clienti",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contabilita_interna_trimestrale",
                columns: table => new
                {
                    id_contabilita_interna_trimestrale = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_anno = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    codice_contabilita = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    primo_trimestre_ultima_ft_vendita = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    primo_trimestre_data_ft = table.Column<DateTime>(type: "datetime2", nullable: true),
                    primo_trimestre_liq_iva_importo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    primo_trimestre_debito_credito = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    primo_trimestre_f24_consegnato = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    primo_trimestre_importo_credito = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    primo_trimestre_importo_debito = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    secondo_trimestre_ultima_ft_vendita = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    secondo_trimestre_data_ft = table.Column<DateTime>(type: "datetime2", nullable: true),
                    secondo_trimestre_liq_iva_importo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    secondo_trimestre_debito_credito = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    secondo_trimestre_f24_consegnato = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    secondo_trimestre_importo_credito = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    secondo_trimestre_importo_debito = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    terzo_trimestre_ultima_ft_vendita = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    terzo_trimestre_data_ft = table.Column<DateTime>(type: "datetime2", nullable: true),
                    terzo_trimestre_liq_iva_importo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    terzo_trimestre_debito_credito = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    terzo_trimestre_f24_consegnato = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    terzo_trimestre_importo_credito = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    terzo_trimestre_importo_debito = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    quarto_trimestre_ultima_ft_vendita = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    quarto_trimestre_data_ft = table.Column<DateTime>(type: "datetime2", nullable: true),
                    quarto_trimestre_liq_iva_importo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    quarto_trimestre_debito_credito = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    quarto_trimestre_f24_consegnato = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    quarto_trimestre_importo_credito = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    quarto_trimestre_importo_debito = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contabilita_interna_trimestrale", x => x.id_contabilita_interna_trimestrale);
                    table.ForeignKey(
                        name: "FK_contabilita_interna_trimestrale_anni_fiscali_id_anno",
                        column: x => x.id_anno,
                        principalTable: "anni_fiscali",
                        principalColumn: "id_anno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contabilita_interna_trimestrale_clienti_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clienti",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proforma_generate",
                columns: table => new
                {
                    id_proforma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_anno_fatturazione = table.Column<int>(type: "int", nullable: false),
                    data_mandato = table.Column<DateTime>(type: "datetime2", nullable: true),
                    importo_mandato_annuo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    tipo_proforma = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    numero_rata = table.Column<int>(type: "int", nullable: false),
                    data_scadenza = table.Column<DateTime>(type: "datetime2", nullable: false),
                    importo_rata = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    inviata = table.Column<bool>(type: "bit", nullable: false),
                    pagata = table.Column<bool>(type: "bit", nullable: false),
                    data_invio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    data_pagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proforma_generate", x => x.id_proforma);
                    table.ForeignKey(
                        name: "FK_proforma_generate_anni_fatturazione_id_anno_fatturazione",
                        column: x => x.id_anno_fatturazione,
                        principalTable: "anni_fatturazione",
                        principalColumn: "id_anno_fatturazione",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proforma_generate_clienti_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clienti",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attivita_lipe_id_anno",
                table: "attivita_lipe",
                column: "id_anno");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_lipe_id_cliente",
                table: "attivita_lipe",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_lipe_id_professionista",
                table: "attivita_lipe",
                column: "id_professionista");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_mod_tr_iva_id_anno",
                table: "attivita_mod_tr_iva",
                column: "id_anno");

            migrationBuilder.CreateIndex(
                name: "IX_attivita_mod_tr_iva_id_cliente",
                table: "attivita_mod_tr_iva",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_contabilita_interna_trimestrale_id_anno",
                table: "contabilita_interna_trimestrale",
                column: "id_anno");

            migrationBuilder.CreateIndex(
                name: "IX_contabilita_interna_trimestrale_id_cliente",
                table: "contabilita_interna_trimestrale",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_proforma_generate_id_anno_fatturazione",
                table: "proforma_generate",
                column: "id_anno_fatturazione");

            migrationBuilder.CreateIndex(
                name: "IX_proforma_generate_id_cliente",
                table: "proforma_generate",
                column: "id_cliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attivita_lipe");

            migrationBuilder.DropTable(
                name: "attivita_mod_tr_iva");

            migrationBuilder.DropTable(
                name: "contabilita_interna_trimestrale");

            migrationBuilder.DropTable(
                name: "proforma_generate");

            migrationBuilder.DropTable(
                name: "anni_fatturazione");

            migrationBuilder.DropColumn(
                name: "cap",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "cf_cliente",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "cf_legale_rappresentante",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "citta",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "data_mandato",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "importo_mandato_annuo",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "indirizzo",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "legale_rappresentante",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "piva_cliente",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "proforma_tipo",
                table: "clienti");

            migrationBuilder.DropColumn(
                name: "provincia",
                table: "clienti");

            migrationBuilder.AlterColumn<decimal>(
                name: "importo_dr_iva",
                table: "attivita_driva",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "importo_acconto_iva",
                table: "attivita_driva",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);
        }
    }
}
