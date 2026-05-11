using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class FixingIndexConflictInTripTripSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location_Longitude",
                schema: "rafeeq",
                table: "TripSites",
                newName: "SiteLocation_Longitude");

            migrationBuilder.RenameColumn(
                name: "Location_Latitude",
                schema: "rafeeq",
                table: "TripSites",
                newName: "SiteLocation_Latitude");

            migrationBuilder.RenameIndex(
                name: "IX_Sites_Location_Latitude_Longitude",
                schema: "rafeeq",
                table: "TripSites",
                newName: "IX_TripSites_SiteLocation_LatLng");

            migrationBuilder.RenameColumn(
                name: "Location_Longitude",
                schema: "rafeeq",
                table: "Trips",
                newName: "UserPosition_Longitude");

            migrationBuilder.RenameColumn(
                name: "Location_Latitude",
                schema: "rafeeq",
                table: "Trips",
                newName: "UserPosition_Latitude");

            migrationBuilder.RenameIndex(
                name: "IX_Sites_Location_Latitude_Longitude",
                schema: "rafeeq",
                table: "Trips",
                newName: "IX_Trips_UserPosition_LatLng");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SiteLocation_Longitude",
                schema: "rafeeq",
                table: "TripSites",
                newName: "Location_Longitude");

            migrationBuilder.RenameColumn(
                name: "SiteLocation_Latitude",
                schema: "rafeeq",
                table: "TripSites",
                newName: "Location_Latitude");

            migrationBuilder.RenameIndex(
                name: "IX_TripSites_SiteLocation_LatLng",
                schema: "rafeeq",
                table: "TripSites",
                newName: "IX_Sites_Location_Latitude_Longitude");

            migrationBuilder.RenameColumn(
                name: "UserPosition_Longitude",
                schema: "rafeeq",
                table: "Trips",
                newName: "Location_Longitude");

            migrationBuilder.RenameColumn(
                name: "UserPosition_Latitude",
                schema: "rafeeq",
                table: "Trips",
                newName: "Location_Latitude");

            migrationBuilder.RenameIndex(
                name: "IX_Trips_UserPosition_LatLng",
                schema: "rafeeq",
                table: "Trips",
                newName: "IX_Sites_Location_Latitude_Longitude");
        }
    }
}
