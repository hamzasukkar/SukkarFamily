using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SukkarFamily.Migrations
{
    /// <inheritdoc />
    public partial class AddNationalNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalNumber",
                table: "persones",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalNumber",
                table: "persones");
        }
    }
}
