using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterLinked.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialMediaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwitterUrl",
                table: "AspNetUsers");
        }
    }
}
