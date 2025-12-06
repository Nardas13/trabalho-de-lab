using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHubProjeto.Migrations
{
    public partial class AddFavoritosCorrect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Favorito",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    IdComprador = table.Column<int>(nullable: false),
                    IdAnuncio = table.Column<int>(nullable: false),
                    DataFavorito = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorito", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Favorito_Comprador_IdComprador",
                        column: x => x.IdComprador,
                        principalTable: "Comprador",
                        principalColumn: "IdComprador",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Favorito_Anuncio_IdAnuncio",
                        column: x => x.IdAnuncio,
                        principalTable: "Anuncio",
                        principalColumn: "IdAnuncio",
                        onDelete: ReferentialAction.Cascade);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_IdComprador",
                table: "Favorito",
                column: "IdComprador"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_IdAnuncio",
                table: "Favorito",
                column: "IdAnuncio"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorito"
            );
        }
    }
}
