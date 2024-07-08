using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FellowshipOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class modifycommunicationcolumnsearchvalue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SearchValue",
                table: "Communications",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
