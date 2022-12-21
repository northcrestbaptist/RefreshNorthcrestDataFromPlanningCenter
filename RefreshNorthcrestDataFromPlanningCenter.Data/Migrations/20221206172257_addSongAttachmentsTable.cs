using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefreshNorthcrestDataFromPlanningCenter.Data.Migrations
{
    public partial class addSongAttachmentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SongAttachments",
                columns: table => new
                {
                    SongAttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    File = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FilePath = table.Column<string>(type: "varchar(500)", nullable: true),
                    FileName = table.Column<string>(type: "varchar(100)", nullable: false),
                    FileType = table.Column<string>(type: "varchar(25)", nullable: false),
                    ContentType = table.Column<string>(type: "varchar(100)", nullable: true),
                    Downloadable = table.Column<bool>(type: "bit", nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "varchar(5000)", nullable: true),
                    HasPreview = table.Column<bool>(type: "bit", nullable: false),
                    PlanSongId = table.Column<int>(type: "int", nullable: false),
                    SermonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongAttachments", x => x.SongAttachmentId);
                    table.ForeignKey(
                        name: "FK_SongAttachments_PlanSongs_PlanSongId",
                        column: x => x.PlanSongId,
                        principalTable: "PlanSongs",
                        principalColumn: "PlanSongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongAttachments_PlanSongId",
                table: "SongAttachments",
                column: "PlanSongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongAttachments");
        }
    }
}
