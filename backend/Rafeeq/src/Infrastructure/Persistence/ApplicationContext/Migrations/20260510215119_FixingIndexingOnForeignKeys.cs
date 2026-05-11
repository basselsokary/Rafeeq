using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class FixingIndexingOnForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_SponsorId",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_SiteId",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_OpeningHours_SiteId",
                schema: "rafeeq",
                table: "Site_OpeningHours",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocalizedContents_OfferId",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportationLocalizedContents_TransportationId",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                column: "TransportationId");

            migrationBuilder.CreateIndex(
                name: "IX_CityLocalizedContents_CityId",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_AttractionId",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "AttractionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SponsorLocalizedContents_SponsorId",
                schema: "rafeeq",
                table: "SponsorLocalizedContents");

            migrationBuilder.DropIndex(
                name: "IX_SiteLocalizedContents_SiteId",
                schema: "rafeeq",
                table: "SiteLocalizedContents");

            migrationBuilder.DropIndex(
                name: "IX_Sites_OpeningHours_SiteId",
                schema: "rafeeq",
                table: "Site_OpeningHours");

            migrationBuilder.DropIndex(
                name: "IX_OfferLocalizedContents_OfferId",
                schema: "rafeeq",
                table: "OfferLocalizedContents");

            migrationBuilder.DropIndex(
                name: "IX_NearestTransportationLocalizedContents_TransportationId",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents");

            migrationBuilder.DropIndex(
                name: "IX_CityLocalizedContents_CityId",
                schema: "rafeeq",
                table: "CityLocalizedContents");

            migrationBuilder.DropIndex(
                name: "IX_AttractionLocalizedContents_AttractionId",
                schema: "rafeeq",
                table: "AttractionLocalizedContents");
        }
    }
}
