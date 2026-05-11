using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class FixingIsClosedInOpeningHoursProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                schema: "rafeeq",
                table: "Site_OpeningHours",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                schema: "rafeeq",
                table: "Site_OpeningHours");
        }
    }
}
