using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class AddingTripsAndVisitedSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_City",
                schema: "rafeeq",
                table: "SponsorLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                schema: "rafeeq",
                table: "SponsorLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Address_Region",
                schema: "rafeeq",
                table: "SponsorLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Address_City",
                schema: "rafeeq",
                table: "SiteLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                schema: "rafeeq",
                table: "SiteLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Address_Region",
                schema: "rafeeq",
                table: "SiteLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Address_City",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Address_Region",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents");

            migrationBuilder.RenameColumn(
                name: "Address_Street",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "EntryFee_Currency",
                schema: "rafeeq",
                table: "Sites",
                newName: "ForeignerPrice_Currency");

            migrationBuilder.RenameColumn(
                name: "EntryFee_Amount",
                schema: "rafeeq",
                table: "Sites",
                newName: "ForeignerPrice_Amount");

            migrationBuilder.RenameColumn(
                name: "TotalReviews",
                schema: "rafeeq",
                table: "Sites",
                newName: "TotalVisits");

            migrationBuilder.RenameColumn(
                name: "Address_Street",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "Address_Street",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                newName: "Address");

            migrationBuilder.AddColumn<decimal>(
                name: "EgyptianPrice_Amount",
                schema: "rafeeq",
                table: "Sites",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EgyptianPrice_Currency",
                schema: "rafeeq",
                table: "Sites",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntryTicket_Notes",
                schema: "rafeeq",
                table: "Sites",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedDurationMinutes",
                schema: "rafeeq",
                table: "Sites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPopular",
                schema: "rafeeq",
                table: "Sites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TicketExists",
                schema: "rafeeq",
                table: "Sites",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalRating",
                schema: "rafeeq",
                table: "Sites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "rafeeq",
                table: "Cities",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                schema: "rafeeq",
                table: "Cities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Trips",
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
                    TouristId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PreferredTransportation = table.Column<int>(type: "int", nullable: false),
                    EstimatedBudget_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EstimatedBudget_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    ActualCost_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ActualCost_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    EstimatedTotalDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    ShareCount = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisitedSites",
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
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    TouristId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitedSites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitedSites_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitedSites_Tourists_TouristId",
                        column: x => x.TouristId,
                        principalSchema: "rafeeq",
                        principalTable: "Tourists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "TripSites",
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
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisited = table.Column<bool>(type: "bit", nullable: false),
                    ActualVisitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDurationMinutes = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripSites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripSites_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripSites_Trips_TripId",
                        column: x => x.TripId,
                        principalSchema: "rafeeq",
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Trips_CreatedAt",
                schema: "rafeeq",
                table: "Trips",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_CreatedBy",
                schema: "rafeeq",
                table: "Trips",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_IsPublic",
                schema: "rafeeq",
                table: "Trips",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_LastModifiedAt",
                schema: "rafeeq",
                table: "Trips",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_LastModifiedBy",
                schema: "rafeeq",
                table: "Trips",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Name",
                schema: "rafeeq",
                table: "Trips",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Status",
                schema: "rafeeq",
                table: "Trips",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TouristId",
                schema: "rafeeq",
                table: "Trips",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_CreatedAt",
                schema: "rafeeq",
                table: "TripSites",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_CreatedBy",
                schema: "rafeeq",
                table: "TripSites",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_LastModifiedAt",
                schema: "rafeeq",
                table: "TripSites",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_LastModifiedBy",
                schema: "rafeeq",
                table: "TripSites",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_SiteId",
                schema: "rafeeq",
                table: "TripSites",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_TripId_DisplayOrder",
                schema: "rafeeq",
                table: "TripSites",
                columns: new[] { "TripId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_TripId_VisitDate",
                schema: "rafeeq",
                table: "TripSites",
                columns: new[] { "TripId", "VisitDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VisitedSites_CreatedAt",
                schema: "rafeeq",
                table: "VisitedSites",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VisitedSites_CreatedBy",
                schema: "rafeeq",
                table: "VisitedSites",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VisitedSites_LastModifiedAt",
                schema: "rafeeq",
                table: "VisitedSites",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VisitedSites_LastModifiedBy",
                schema: "rafeeq",
                table: "VisitedSites",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VisitedSites_SiteId",
                schema: "rafeeq",
                table: "VisitedSites",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitedSites_TouristId_SiteId",
                schema: "rafeeq",
                table: "VisitedSites",
                columns: new[] { "TouristId", "SiteId" },
                unique: true,
                filter: "[TouristId] IS NOT NULL AND [SiteId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripNotes",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "TripSites",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "VisitedSites",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Trips",
                schema: "rafeeq");

            migrationBuilder.DropColumn(
                name: "EgyptianPrice_Amount",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "EgyptianPrice_Currency",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "EntryTicket_Notes",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "EstimatedDurationMinutes",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "IsPopular",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "TicketExists",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "TotalRating",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                schema: "rafeeq",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "Address",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                newName: "Address_Street");

            migrationBuilder.RenameColumn(
                name: "ForeignerPrice_Currency",
                schema: "rafeeq",
                table: "Sites",
                newName: "EntryFee_Currency");

            migrationBuilder.RenameColumn(
                name: "ForeignerPrice_Amount",
                schema: "rafeeq",
                table: "Sites",
                newName: "EntryFee_Amount");

            migrationBuilder.RenameColumn(
                name: "TotalVisits",
                schema: "rafeeq",
                table: "Sites",
                newName: "TotalReviews");

            migrationBuilder.RenameColumn(
                name: "Address",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                newName: "Address_Street");

            migrationBuilder.RenameColumn(
                name: "Address",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                newName: "Address_Street");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Region",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Region",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Region",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "rafeeq",
                table: "Cities",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);
        }
    }
}
