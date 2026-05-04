using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class AddingArtifactEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryTicket_Notes",
                schema: "rafeeq",
                table: "Sites");

            migrationBuilder.AddColumn<string>(
                name: "EntryTicketNotes",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Artifacts",
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
                    MainImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artifacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artifacts_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtifactImages",
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
                    StorageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ArtifactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtifactImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtifactImages_Artifacts_ArtifactId",
                        column: x => x.ArtifactId,
                        principalSchema: "rafeeq",
                        principalTable: "Artifacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtifactLocalizedContents",
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
                    Language = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    ArtifactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtifactLocalizedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtifactLocalizedContents_Artifacts_ArtifactId",
                        column: x => x.ArtifactId,
                        principalSchema: "rafeeq",
                        principalTable: "Artifacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactImages_ArtifactId_DisplayOrder",
                schema: "rafeeq",
                table: "ArtifactImages",
                columns: new[] { "ArtifactId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactImages_ArtifactId_IsMain",
                schema: "rafeeq",
                table: "ArtifactImages",
                columns: new[] { "ArtifactId", "IsMain" });

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactImages_CreatedAt",
                schema: "rafeeq",
                table: "ArtifactImages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactImages_CreatedBy",
                schema: "rafeeq",
                table: "ArtifactImages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactImages_LastModifiedAt",
                schema: "rafeeq",
                table: "ArtifactImages",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactImages_LastModifiedBy",
                schema: "rafeeq",
                table: "ArtifactImages",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactLocalizedContents_ArtifactId_Language",
                schema: "rafeeq",
                table: "ArtifactLocalizedContents",
                columns: new[] { "ArtifactId", "Language" },
                unique: true,
                filter: "[ArtifactId] IS NOT NULL AND [Language] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactLocalizedContents_CreatedAt",
                schema: "rafeeq",
                table: "ArtifactLocalizedContents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactLocalizedContents_CreatedBy",
                schema: "rafeeq",
                table: "ArtifactLocalizedContents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactLocalizedContents_LastModifiedAt",
                schema: "rafeeq",
                table: "ArtifactLocalizedContents",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactLocalizedContents_LastModifiedBy",
                schema: "rafeeq",
                table: "ArtifactLocalizedContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactLocalizedContents_Name",
                schema: "rafeeq",
                table: "ArtifactLocalizedContents",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_CreatedAt",
                schema: "rafeeq",
                table: "Artifacts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_CreatedBy",
                schema: "rafeeq",
                table: "Artifacts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_DisplayOrder",
                schema: "rafeeq",
                table: "Artifacts",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_LastModifiedAt",
                schema: "rafeeq",
                table: "Artifacts",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_LastModifiedBy",
                schema: "rafeeq",
                table: "Artifacts",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_SiteId",
                schema: "rafeeq",
                table: "Artifacts",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_Type",
                schema: "rafeeq",
                table: "Artifacts",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtifactImages",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "ArtifactLocalizedContents",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Artifacts",
                schema: "rafeeq");

            migrationBuilder.DropColumn(
                name: "EntryTicketNotes",
                schema: "rafeeq",
                table: "SiteLocalizedContents");

            migrationBuilder.AddColumn<string>(
                name: "EntryTicket_Notes",
                schema: "rafeeq",
                table: "Sites",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }
    }
}
