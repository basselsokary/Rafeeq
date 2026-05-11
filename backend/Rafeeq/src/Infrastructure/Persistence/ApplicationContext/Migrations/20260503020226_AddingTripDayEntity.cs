using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class AddingTripDayEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripSites_Trips_TripId",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropTable(
                name: "TripNotes",
                schema: "rafeeq");

            migrationBuilder.DropIndex(
                name: "IX_Trips_IsPublic",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ActualDurationMinutes",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "PreferredTransportation",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "TripId",
                schema: "rafeeq",
                table: "TripSites",
                newName: "TripDayId");

            migrationBuilder.RenameColumn(
                name: "ShareCount",
                schema: "rafeeq",
                table: "Trips",
                newName: "TotalSites");

            migrationBuilder.AddColumn<double>(
                name: "Location_Latitude",
                schema: "rafeeq",
                table: "TripSites",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Location_Longitude",
                schema: "rafeeq",
                table: "TripSites",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "rafeeq",
                table: "TripSites",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price_Amount",
                schema: "rafeeq",
                table: "TripSites",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Price_Currency",
                schema: "rafeeq",
                table: "TripSites",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "rafeeq",
                table: "TripSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                schema: "rafeeq",
                table: "Trips",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<double>(
                name: "Location_Latitude",
                schema: "rafeeq",
                table: "Trips",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Location_Longitude",
                schema: "rafeeq",
                table: "Trips",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PreferredSiteTypes",
                schema: "rafeeq",
                table: "Trips",
                type: "nvarchar(1024)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                schema: "rafeeq",
                table: "Trips",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "Tolerance",
                schema: "rafeeq",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TripDay",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Price_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    TotalSites = table.Column<int>(type: "int", nullable: false),
                    DayTotalDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripDay_Trips_TripId",
                        column: x => x.TripId,
                        principalSchema: "rafeeq",
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Location_Latitude_Longitude",
                schema: "rafeeq",
                table: "TripSites",
                columns: new[] { "Location_Latitude", "Location_Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Location_Latitude_Longitude",
                schema: "rafeeq",
                table: "Trips",
                columns: new[] { "Location_Latitude", "Location_Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_TripDay_CreatedAt",
                schema: "rafeeq",
                table: "TripDay",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TripDay_CreatedBy",
                schema: "rafeeq",
                table: "TripDay",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TripDay_LastModifiedAt",
                schema: "rafeeq",
                table: "TripDay",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TripDay_LastModifiedBy",
                schema: "rafeeq",
                table: "TripDay",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TripDays_Date",
                schema: "rafeeq",
                table: "TripDay",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_TripDays_TripId_Date",
                schema: "rafeeq",
                table: "TripDay",
                columns: new[] { "TripId", "Date" },
                unique: true,
                filter: "[TripId] IS NOT NULL AND [Date] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TripSites_TripDay_TripDayId",
                schema: "rafeeq",
                table: "TripSites",
                column: "TripDayId",
                principalSchema: "rafeeq",
                principalTable: "TripDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripSites_TripDay_TripDayId",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropTable(
                name: "TripDay",
                schema: "rafeeq");

            migrationBuilder.DropIndex(
                name: "IX_Sites_Location_Latitude_Longitude",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_Location_Latitude_Longitude",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Location_Latitude",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "Location_Longitude",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "Price_Amount",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "Price_Currency",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "rafeeq",
                table: "TripSites");

            migrationBuilder.DropColumn(
                name: "EndTime",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Location_Latitude",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Location_Longitude",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "PreferredSiteTypes",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Tolerance",
                schema: "rafeeq",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "TripDayId",
                schema: "rafeeq",
                table: "TripSites",
                newName: "TripId");

            migrationBuilder.RenameColumn(
                name: "TotalSites",
                schema: "rafeeq",
                table: "Trips",
                newName: "ShareCount");

            migrationBuilder.AddColumn<int>(
                name: "ActualDurationMinutes",
                schema: "rafeeq",
                table: "TripSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "rafeeq",
                table: "TripSites",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                schema: "rafeeq",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PreferredTransportation",
                schema: "rafeeq",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TripNotes",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedByName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripNotes_Trips_TripId",
                        column: x => x.TripId,
                        principalSchema: "rafeeq",
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_IsPublic",
                schema: "rafeeq",
                table: "Trips",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_TripNotes_CreatedAt",
                schema: "rafeeq",
                table: "TripNotes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TripNotes_CreatedBy",
                schema: "rafeeq",
                table: "TripNotes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TripNotes_LastModifiedAt",
                schema: "rafeeq",
                table: "TripNotes",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TripNotes_LastModifiedBy",
                schema: "rafeeq",
                table: "TripNotes",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TripNotes_TripId",
                schema: "rafeeq",
                table: "TripNotes",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripSites_Trips_TripId",
                schema: "rafeeq",
                table: "TripSites",
                column: "TripId",
                principalSchema: "rafeeq",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
