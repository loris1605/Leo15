using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class _23032026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Person_FirstName",
                table: "People",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Person_SurName",
                table: "People",
                column: "SurName");

            migrationBuilder.Sql("UPDATE Settings SET Version = 2 WHERE Id = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Person_FirstName",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_Person_SurName",
                table: "People");

            migrationBuilder.Sql("UPDATE Settings SET Version = 1 WHERE Id = 1;");
        }
    }
}
