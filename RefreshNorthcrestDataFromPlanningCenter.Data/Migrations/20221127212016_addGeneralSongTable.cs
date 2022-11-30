using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefreshNorthcrestDataFromPlanningCenter.Data.Migrations
{
    public partial class addGeneralSongTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralSongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    ArrangementId = table.Column<int>(type: "int", nullable: false),
                    SongName = table.Column<string>(type: "varchar(500)", nullable: true),
                    ArrangementName = table.Column<string>(type: "varchar(1000)", nullable: true),
                    Author = table.Column<string>(type: "varchar(2000)", nullable: true),
                    Copyright = table.Column<string>(type: "varchar(2000)", nullable: true),
                    Length = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "varchar(5000)", nullable: true),
                    LastScheduledDateTime = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Speaker = table.Column<string>(type: "varchar(1000)", nullable: true),
                    Title = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSongs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralSongs");
        }
    }
}
