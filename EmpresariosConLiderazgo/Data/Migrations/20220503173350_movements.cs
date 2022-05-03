using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpresariosConLiderazgo.Data.Migrations
{
    public partial class movements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBalanceProduct = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateMovement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BalanceBefore = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BalanceAfter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movements", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movements");
        }
    }
}
