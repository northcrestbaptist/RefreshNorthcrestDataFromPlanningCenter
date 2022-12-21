using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefreshNorthcrestDataFromPlanningCenter.Data.Migrations
{
    public partial class removeSermonIdFromSongAttachmentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SermonId",
                table: "SongAttachments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SermonId",
                table: "SongAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
