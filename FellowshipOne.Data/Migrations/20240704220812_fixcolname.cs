using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FellowshipOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixcolname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OccumpationName",
                table: "Persons");

            migrationBuilder.AddColumn<string>(
                name: "OccupationName",
                table: "Persons",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Addresses",
                type: "varchar(100)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OccupationName",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Addresses");
        }
    }
}
