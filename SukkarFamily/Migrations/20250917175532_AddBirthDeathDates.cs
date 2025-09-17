using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SukkarFamily.Migrations
{
    /// <inheritdoc />
    public partial class AddBirthDeathDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "persones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfDeath",
                table: "persones",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "persones");

            migrationBuilder.DropColumn(
                name: "DateOfDeath",
                table: "persones");
        }
    }
}
