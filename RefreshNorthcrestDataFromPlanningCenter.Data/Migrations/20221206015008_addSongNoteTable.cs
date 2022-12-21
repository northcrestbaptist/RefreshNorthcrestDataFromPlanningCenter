using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefreshNorthcrestDataFromPlanningCenter.Data.Migrations
{
    public partial class addSongNoteTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SongNotes",
                columns: table => new
                {
                    SongNoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "varchar(50)", nullable: true),
                    Content = table.Column<string>(type: "varchar(1000)", nullable: true),
                    PlanSongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongNotes", x => x.SongNoteId);
                    table.ForeignKey(
                        name: "FK_SongNotes_PlanSongs_PlanSongId",
                        column: x => x.PlanSongId,
                        principalTable: "PlanSongs",
                        principalColumn: "PlanSongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongNotes_PlanSongId",
                table: "SongNotes",
                column: "PlanSongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongNotes");
        }
    }
}
