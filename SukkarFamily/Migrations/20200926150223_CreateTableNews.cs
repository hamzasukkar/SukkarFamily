using Microsoft.EntityFrameworkCore.Migrations;

namespace SukkarFamily.Migrations
{
    public partial class CreateTableNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    ImgUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "persones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    image = table.Column<string>(nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    Generation = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_persones_persones_ParentId",
                        column: x => x.ParentId,
                        principalTable: "persones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_persones_ParentId",
                table: "persones",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "persones");
        }
    }
}
