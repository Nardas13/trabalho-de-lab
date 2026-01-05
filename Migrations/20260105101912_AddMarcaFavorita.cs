using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHubProjeto.Migrations
{
    /// <inheritdoc />
    public partial class AddMarcaFavorita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "MarcaFavorita",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdComprador = table.Column<int>(type: "int", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarcaFavorita", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarcaFavorita_Comprador",
                        column: x => x.IdComprador,
                        principalTable: "Comprador",
                        principalColumn: "IdComprador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarcaFavorita_IdComprador",
                table: "MarcaFavorita",
                column: "IdComprador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarcaFavorita");

            migrationBuilder.AddColumn<string>(
                name: "MarcaFavorita",
                table: "Comprador",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificacoesAtivas",
                table: "Comprador",
                type: "bit",
                nullable: false,
                defaultValue: true);

        }
    }
}
