using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class UserManagmentFixedAndRemoveUniquenessFromAttractionLocalizedContentName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttractionLocalizedContents_Name",
                schema: "rafeeq",
                table: "AttractionLocalizedContents");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "rafeeq",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TotalReviews",
                schema: "rafeeq",
                table: "Tourists",
                newName: "TotalRatings");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "rafeeq",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChangedAt",
                schema: "rafeeq",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                schema: "rafeeq",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_Name",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttractionLocalizedContents_Name",
                schema: "rafeeq",
                table: "AttractionLocalizedContents");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "rafeeq",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastPasswordChangedAt",
                schema: "rafeeq",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                schema: "rafeeq",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TotalRatings",
                schema: "rafeeq",
                table: "Tourists",
                newName: "TotalReviews");

            migrationBuilder.AddColumn<byte>(
                name: "Role",
                schema: "rafeeq",
                table: "Users",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_Name",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
