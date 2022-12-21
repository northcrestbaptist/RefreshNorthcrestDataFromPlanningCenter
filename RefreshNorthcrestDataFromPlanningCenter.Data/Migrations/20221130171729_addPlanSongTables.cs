using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefreshNorthcrestDataFromPlanningCenter.Data.Migrations
{
    public partial class addPlanSongTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plan_ForSongs",
                columns: table => new
                {
                    Plan_ForSongsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", nullable: true),
                    PlanDateTime = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan_ForSongs", x => x.Plan_ForSongsId);
                });

            migrationBuilder.CreateTable(
                name: "PlanSongs",
                columns: table => new
                {
                    PlanSongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    ArrangementId = table.Column<int>(type: "int", nullable: false),
                    SongName = table.Column<string>(type: "varchar(500)", nullable: true),
                    ArrangementName = table.Column<string>(type: "varchar(1000)", nullable: true),
                    Author = table.Column<string>(type: "varchar(2000)", nullable: true),
                    Copyright = table.Column<string>(type: "varchar(2000)", nullable: true),
                    Length = table.Column<int>(type: "int", nullable: false),
                    KeyName = table.Column<string>(type: "varchar(25)", nullable: true),
                    Description = table.Column<string>(type: "varchar(2000)", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "varchar(5000)", nullable: true),
                    Plan_ForSongsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanSongs", x => x.PlanSongId);
                    table.ForeignKey(
                        name: "FK_PlanSongs_Plan_ForSongs_Plan_ForSongsId",
                        column: x => x.Plan_ForSongsId,
                        principalTable: "Plan_ForSongs",
                        principalColumn: "Plan_ForSongsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanSongs_Plan_ForSongsId",
                table: "PlanSongs",
                column: "Plan_ForSongsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanSongs");

            migrationBuilder.DropTable(
                name: "Plan_ForSongs");
        }
    }
}
