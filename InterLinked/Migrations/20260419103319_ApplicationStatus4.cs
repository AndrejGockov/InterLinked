using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Linker.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationStatus4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvPath",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "CvPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvPath",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CvPath",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
