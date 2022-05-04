using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpresariosConLiderazgo.Data.Migrations
{
    public partial class changemovement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Movements");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Movements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Movements");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Movements",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
