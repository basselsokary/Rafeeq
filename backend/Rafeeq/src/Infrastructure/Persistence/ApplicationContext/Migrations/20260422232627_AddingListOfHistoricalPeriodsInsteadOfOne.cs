using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class AddingListOfHistoricalPeriodsInsteadOfOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attractions_HistoricalPeriod",
                schema: "rafeeq",
                table: "Attractions");

            migrationBuilder.DropColumn(
                name: "HistoricalPeriod",
                schema: "rafeeq",
                table: "Attractions");

            migrationBuilder.AddColumn<string>(
                name: "HistoricalPeriods",
                schema: "rafeeq",
                table: "Attractions",
                type: "nvarchar(1024)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HistoricalPeriods",
                schema: "rafeeq",
                table: "Attractions");

            migrationBuilder.AddColumn<int>(
                name: "HistoricalPeriod",
                schema: "rafeeq",
                table: "Attractions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_HistoricalPeriod",
                schema: "rafeeq",
                table: "Attractions",
                column: "HistoricalPeriod");
        }
    }
}
