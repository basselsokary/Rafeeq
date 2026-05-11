using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class FixingMaxLengths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "Trips",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "rafeeq",
                table: "Reviews",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "ContentReports",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "ArtifactLocalizedContents",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "Trips",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "rafeeq",
                table: "Reviews",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "ContentReports",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "rafeeq",
                table: "ArtifactLocalizedContents",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);
        }
    }
}
