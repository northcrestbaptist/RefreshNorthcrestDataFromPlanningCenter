using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefreshNorthcrestDataFromPlanningCenter.Data.Migrations
{
    public partial class replacedNotesColWithThemesColInGeneralSongsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "GeneralSongs");

            migrationBuilder.AddColumn<string>(
                name: "Themes",
                table: "GeneralSongs",
                type: "varchar(2000)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Themes",
                table: "GeneralSongs");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "GeneralSongs",
                type: "varchar(5000)",
                nullable: true);
        }
    }
}
