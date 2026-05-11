using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingSiteArtifactRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artifacts_Sites_SiteId",
                schema: "rafeeq",
                table: "Artifacts");

            migrationBuilder.AlterColumn<Guid>(
                name: "SiteId",
                schema: "rafeeq",
                table: "Artifacts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Artifacts_Sites_SiteId",
                schema: "rafeeq",
                table: "Artifacts",
                column: "SiteId",
                principalSchema: "rafeeq",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artifacts_Sites_SiteId",
                schema: "rafeeq",
                table: "Artifacts");

            migrationBuilder.AlterColumn<Guid>(
                name: "SiteId",
                schema: "rafeeq",
                table: "Artifacts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Artifacts_Sites_SiteId",
                schema: "rafeeq",
                table: "Artifacts",
                column: "SiteId",
                principalSchema: "rafeeq",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
