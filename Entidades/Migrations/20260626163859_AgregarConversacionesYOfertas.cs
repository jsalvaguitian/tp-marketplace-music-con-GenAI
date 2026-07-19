using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entidades.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConversacionesYOfertas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Publicaciones_PublicacionId",
                table: "Mensajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Usuarios_DestinatarioId",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_DestinatarioId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "DestinatarioId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "EsPrivado",
                table: "Mensajes");

            migrationBuilder.RenameColumn(
                name: "PublicacionId",
                table: "Mensajes",
                newName: "ConversacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Mensajes_PublicacionId",
                table: "Mensajes",
                newName: "IX_Mensajes_ConversacionId");

            migrationBuilder.CreateTable(
                name: "Ofertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductoOfrecido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    PublicacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ofertas_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalTable: "Publicaciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ofertas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Conversaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicacionId = table.Column<int>(type: "int", nullable: false),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    CompradorId = table.Column<int>(type: "int", nullable: false),
                    OfertaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversaciones_Ofertas_OfertaId",
                        column: x => x.OfertaId,
                        principalTable: "Ofertas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversaciones_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalTable: "Publicaciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversaciones_Usuarios_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversaciones_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_CompradorId",
                table: "Conversaciones",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_OfertaId",
                table: "Conversaciones",
                column: "OfertaId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_PublicacionId",
                table: "Conversaciones",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_VendedorId",
                table: "Conversaciones",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_PublicacionId",
                table: "Ofertas",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_UsuarioId",
                table: "Ofertas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Conversaciones_ConversacionId",
                table: "Mensajes",
                column: "ConversacionId",
                principalTable: "Conversaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Conversaciones_ConversacionId",
                table: "Mensajes");

            migrationBuilder.DropTable(
                name: "Conversaciones");

            migrationBuilder.DropTable(
                name: "Ofertas");

            migrationBuilder.RenameColumn(
                name: "ConversacionId",
                table: "Mensajes",
                newName: "PublicacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Mensajes_ConversacionId",
                table: "Mensajes",
                newName: "IX_Mensajes_PublicacionId");

            migrationBuilder.AddColumn<int>(
                name: "DestinatarioId",
                table: "Mensajes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsPrivado",
                table: "Mensajes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_DestinatarioId",
                table: "Mensajes",
                column: "DestinatarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Publicaciones_PublicacionId",
                table: "Mensajes",
                column: "PublicacionId",
                principalTable: "Publicaciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Usuarios_DestinatarioId",
                table: "Mensajes",
                column: "DestinatarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
