using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingSiteAndSponsorStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sponsors_IsActive",
                schema: "rafeeq",
                table: "Sponsors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "rafeeq",
                table: "Sponsors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "rafeeq",
                table: "Sponsors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_Status",
                schema: "rafeeq",
                table: "Sponsors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_IsHiddenGem",
                schema: "rafeeq",
                table: "Sites",
                column: "IsHiddenGem");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_IsPopular",
                schema: "rafeeq",
                table: "Sites",
                column: "IsPopular");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sponsors_Status",
                schema: "rafeeq",
                table: "Sponsors");

            migrationBuilder.DropIndex(
                name: "IX_Sites_IsHiddenGem",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_IsPopular",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "rafeeq",
                table: "Sponsors");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "rafeeq",
                table: "Sponsors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "rafeeq",
                table: "Sites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_IsActive",
                schema: "rafeeq",
                table: "Sponsors",
                column: "IsActive");
        }
    }
}
