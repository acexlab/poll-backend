using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pollbackend.Migrations
{
    /// <inheritdoc />
    public partial class AddShowResultsToPoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowResults",
                table: "Polls",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowResults",
                table: "Polls");
        }
    }
}
