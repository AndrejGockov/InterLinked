using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterLinked.Migrations
{
    /// <inheritdoc />
    public partial class postModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationLink",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Salary",
                table: "Post",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkplaceType",
                table: "Post",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "LocationLink",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "WorkplaceType",
                table: "Post");
        }
    }
}
