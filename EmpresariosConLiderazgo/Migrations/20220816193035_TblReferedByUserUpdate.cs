using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpresariosConLiderazgo.Migrations
{
    public partial class TblReferedByUserUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "referedUserId",
                table: "ReferedByUser",
                newName: "ReferedUserId");

            migrationBuilder.RenameColumn(
                name: "effectiveContact",
                table: "ReferedByUser",
                newName: "InvestDone");

            migrationBuilder.AddColumn<bool>(
                name: "Accepted",
                table: "ReferedByUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountToRefer",
                table: "ReferedByUser",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "ReferedByUser");

            migrationBuilder.DropColumn(
                name: "AmountToRefer",
                table: "ReferedByUser");

            migrationBuilder.RenameColumn(
                name: "ReferedUserId",
                table: "ReferedByUser",
                newName: "referedUserId");

            migrationBuilder.RenameColumn(
                name: "InvestDone",
                table: "ReferedByUser",
                newName: "effectiveContact");
        }
    }
}
