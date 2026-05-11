using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTripAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TripSites_TripId_VisitDate",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "EndTime",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "VisitDate",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "rafeeq",
                table: "TripSites",
                newName: "SiteName");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                schema: "rafeeq",
                table: "TripSites",
                newName: "VisitOrder");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                schema: "rafeeq",
                table: "Trips",
                newName: "DailyStartTime");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "rafeeq",
                table: "Trips",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                schema: "rafeeq",
                table: "Trips",
                newName: "DailyEndTime");

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                schema: "rafeeq",
                table: "TripSites",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PlannedArrivalTime",
                schema: "rafeeq",
                table: "TripSites",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "SiteImageUrl",
                schema: "rafeeq",
                table: "TripSites",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                schema: "rafeeq",
                table: "Trips",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                schema: "rafeeq",
                table: "Trips",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "DayNumber",
                schema: "rafeeq",
                table: "TripDay",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_TripId_PlannedArrivalTime",
                schema: "rafeeq",
                table: "TripSites",
                columns: new[] { "TripDayId", "PlannedArrivalTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartDate",
                schema: "rafeeq",
                table: "Trips",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TripSites_TripId_PlannedArrivalTime",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropIndex(
                name: "IX_Trips_StartDate",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "CityName",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "PlannedArrivalTime",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "SiteImageUrl",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "DayNumber",
                schema: "rafeeq",
                table: "TripDay");

            migrationBuilder.RenameColumn(
                name: "VisitOrder",
                schema: "rafeeq",
                table: "TripSites",
                newName: "DisplayOrder");

            migrationBuilder.RenameColumn(
                name: "SiteName",
                schema: "rafeeq",
                table: "TripSites",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "rafeeq",
                table: "Trips",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DailyStartTime",
                schema: "rafeeq",
                table: "Trips",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "DailyEndTime",
                schema: "rafeeq",
                table: "Trips",
                newName: "EndTime");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                schema: "rafeeq",
                table: "TripSites",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                schema: "rafeeq",
                table: "TripSites",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VisitDate",
                schema: "rafeeq",
                table: "TripSites",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                schema: "rafeeq",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                schema: "rafeeq",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_TripId_VisitDate",
                schema: "rafeeq",
                table: "TripSites",
                columns: new[] { "TripDayId", "VisitDate" });
        }
    }
}
