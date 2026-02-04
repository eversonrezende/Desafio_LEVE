using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desafio.Leve.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByIdToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "AspNetUsers");
        }
    }
}
