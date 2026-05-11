using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class AddingStoredFileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoredFileId",
                schema: "rafeeq",
                table: "SponsorImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StoredFileId",
                schema: "rafeeq",
                table: "SiteImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StoredFileId",
                schema: "rafeeq",
                table: "Cities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoredFileId",
                schema: "rafeeq",
                table: "AttractionImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "StoredFiles",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    FirstUploadedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SponsorImages_StoredFileId",
                schema: "rafeeq",
                table: "SponsorImages",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteImages_StoredFileId",
                schema: "rafeeq",
                table: "SiteImages",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StoredFileId",
                schema: "rafeeq",
                table: "Cities",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionImages_StoredFileId",
                schema: "rafeeq",
                table: "AttractionImages",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_Hash",
                schema: "rafeeq",
                table: "StoredFiles",
                column: "Hash");

            migrationBuilder.AddForeignKey(
                name: "FK_AttractionImages_StoredFiles_StoredFileId",
                schema: "rafeeq",
                table: "AttractionImages",
                column: "StoredFileId",
                principalSchema: "rafeeq",
                principalTable: "StoredFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_StoredFiles_StoredFileId",
                schema: "rafeeq",
                table: "Cities",
                column: "StoredFileId",
                principalSchema: "rafeeq",
                principalTable: "StoredFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteImages_StoredFiles_StoredFileId",
                schema: "rafeeq",
                table: "SiteImages",
                column: "StoredFileId",
                principalSchema: "rafeeq",
                principalTable: "StoredFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SponsorImages_StoredFiles_StoredFileId",
                schema: "rafeeq",
                table: "SponsorImages",
                column: "StoredFileId",
                principalSchema: "rafeeq",
                principalTable: "StoredFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttractionImages_StoredFiles_StoredFileId",
                schema: "rafeeq",
                table: "AttractionImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_StoredFiles_StoredFileId",
                schema: "rafeeq",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteImages_StoredFiles_StoredFileId",
                schema: "rafeeq",
                table: "SiteImages");

            migrationBuilder.DropForeignKey(
                name: "FK_SponsorImages_StoredFiles_StoredFileId",
                schema: "rafeeq",
                table: "SponsorImages");

            migrationBuilder.DropTable(
                name: "StoredFiles",
                schema: "rafeeq");

            migrationBuilder.DropIndex(
                name: "IX_SponsorImages_StoredFileId",
                schema: "rafeeq",
                table: "SponsorImages");

            migrationBuilder.DropIndex(
                name: "IX_SiteImages_StoredFileId",
                schema: "rafeeq",
                table: "SiteImages");

            migrationBuilder.DropIndex(
                name: "IX_Cities_StoredFileId",
                schema: "rafeeq",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_AttractionImages_StoredFileId",
                schema: "rafeeq",
                table: "AttractionImages");

            migrationBuilder.DropColumn(
                name: "StoredFileId",
                schema: "rafeeq",
                table: "SponsorImages");

            migrationBuilder.DropColumn(
                name: "StoredFileId",
                schema: "rafeeq",
                table: "SiteImages");

            migrationBuilder.DropColumn(
                name: "StoredFileId",
                schema: "rafeeq",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "StoredFileId",
                schema: "rafeeq",
                table: "AttractionImages");
        }
    }
}
