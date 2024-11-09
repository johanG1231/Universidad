using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _123.Migrations
{
    /// <inheritdoc />
    public partial class AddContenidoPersonalizadoToProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContenidoPersonalizado",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContenidoPersonalizado",
                table: "Productos");
        }
    }
}
