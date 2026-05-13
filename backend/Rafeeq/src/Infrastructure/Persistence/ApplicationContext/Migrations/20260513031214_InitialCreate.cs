using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.ApplicationContext.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "rafeeq");

            migrationBuilder.CreateTable(
                name: "ContentReports",
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
                    ReportedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ActionTaken = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sponsors",
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
                    Type = table.Column<int>(type: "int", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Location_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    Location_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MainImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    TotalRedemptions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                });

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
                    ReferenceCount = table.Column<int>(type: "int", nullable: false),
                    FirstUploadedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tourists",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalTrips = table.Column<int>(type: "int", nullable: false),
                    TotalReviews = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tourists", x => x.Id);
                });

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
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DailyStartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    DailyEndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserPosition_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    UserPosition_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    Tolerance = table.Column<int>(type: "int", nullable: true),
                    EstimatedBudget_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EstimatedBudget_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    ActualCost_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ActualCost_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    EstimatedTotalDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalSites = table.Column<int>(type: "int", nullable: false),
                    PreferredSiteTypes = table.Column<string>(type: "nvarchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "rafeeq",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
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
                    SponsorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Price_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RedemptionCount = table.Column<int>(type: "int", nullable: false),
                    MaxRedemptions = table.Column<int>(type: "int", nullable: true),
                    PromoCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offers_Sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalSchema: "rafeeq",
                        principalTable: "Sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SponsorLocalizedContents",
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
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SponsorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorLocalizedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorLocalizedContents_Sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalSchema: "rafeeq",
                        principalTable: "Sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
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
                    Location_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    Location_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    StoredFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StorageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TotalSites = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalSchema: "rafeeq",
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SponsorImages",
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
                    StoredFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    SponsorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorImages_Sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalSchema: "rafeeq",
                        principalTable: "Sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SponsorImages_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalSchema: "rafeeq",
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Price_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    DayTotalDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalSites = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "rafeeq",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "rafeeq",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "rafeeq",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "rafeeq",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "rafeeq",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "rafeeq",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "rafeeq",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "rafeeq",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferLocalizedContents",
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
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferLocalizedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferLocalizedContents_Offers_OfferId",
                        column: x => x.OfferId,
                        principalSchema: "rafeeq",
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityLocalizedContents",
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
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityLocalizedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CityLocalizedContents_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "rafeeq",
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
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
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Location_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    Location_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    EgyptianPrice_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EgyptianPrice_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    ForeignerPrice_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ForeignerPrice_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    TicketExists = table.Column<bool>(type: "bit", nullable: true),
                    IsFree = table.Column<bool>(type: "bit", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MainImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalVisits = table.Column<int>(type: "int", nullable: false),
                    TotalRating = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsHiddenGem = table.Column<bool>(type: "bit", nullable: false),
                    IsPopular = table.Column<bool>(type: "bit", nullable: false),
                    Facilities = table.Column<string>(type: "nvarchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sites_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "rafeeq",
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Attractions",
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
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    Location_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: true),
                    Location_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: true),
                    MainImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    HistoricalPeriods = table.Column<string>(type: "nvarchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attractions_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favourites",
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
                    Notes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    TouristId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favourites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favourites_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favourites_Tourists_TouristId",
                        column: x => x.TouristId,
                        principalSchema: "rafeeq",
                        principalTable: "Tourists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NearestTransportations",
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
                    Type = table.Column<int>(type: "int", nullable: false),
                    Location_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    Location_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    DistanceKm = table.Column<double>(type: "float", nullable: false),
                    IsOperational = table.Column<bool>(type: "bit", nullable: false),
                    HasAccessibility = table.Column<bool>(type: "bit", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NearestTransportations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NearestTransportations_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
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
                    TouristId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HelpfulCount = table.Column<int>(type: "int", nullable: false),
                    NotHelpfulCount = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Tourists_TouristId",
                        column: x => x.TouristId,
                        principalSchema: "rafeeq",
                        principalTable: "Tourists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Site_OpeningHours",
                schema: "rafeeq",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site_OpeningHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Site_OpeningHours_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteImages",
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
                    StoredFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteImages_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiteImages_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalSchema: "rafeeq",
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteLocalizedContents",
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
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EntryTicketNotes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteLocalizedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteLocalizedContents_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "rafeeq",
                        principalTable: "Sites",
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
                    SiteName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SiteImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SiteLocation_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    SiteLocation_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: false),
                    Price_Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Price_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PlannedArrivalTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    VisitOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisited = table.Column<bool>(type: "bit", nullable: false),
                    ActualVisitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TripDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                        name: "FK_TripSites_TripDay_TripDayId",
                        column: x => x.TripDayId,
                        principalSchema: "rafeeq",
                        principalTable: "TripDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    TouristId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
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
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "AttractionImages",
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
                    StoredFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    AttractionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttractionImages_Attractions_AttractionId",
                        column: x => x.AttractionId,
                        principalSchema: "rafeeq",
                        principalTable: "Attractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttractionImages_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalSchema: "rafeeq",
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttractionLocalizedContents",
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
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    LocationDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    AttractionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionLocalizedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttractionLocalizedContents_Attractions_AttractionId",
                        column: x => x.AttractionId,
                        principalSchema: "rafeeq",
                        principalTable: "Attractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NearestTransportationLocalizedContents",
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
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TransportationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NearestTransportationLocalizedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NearestTransportationLocalizedContents_NearestTransportations_TransportationId",
                        column: x => x.TransportationId,
                        principalSchema: "rafeeq",
                        principalTable: "NearestTransportations",
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
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

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

            migrationBuilder.CreateIndex(
                name: "IX_AttractionImages_AttractionId_DisplayOrder",
                schema: "rafeeq",
                table: "AttractionImages",
                columns: new[] { "AttractionId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_AttractionImages_AttractionId_IsMain",
                schema: "rafeeq",
                table: "AttractionImages",
                columns: new[] { "AttractionId", "IsMain" });

            migrationBuilder.CreateIndex(
                name: "IX_AttractionImages_CreatedAt",
                schema: "rafeeq",
                table: "AttractionImages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionImages_CreatedBy",
                schema: "rafeeq",
                table: "AttractionImages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionImages_LastModifiedAt",
                schema: "rafeeq",
                table: "AttractionImages",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionImages_LastModifiedBy",
                schema: "rafeeq",
                table: "AttractionImages",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionImages_StoredFileId",
                schema: "rafeeq",
                table: "AttractionImages",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_AttractionId",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "AttractionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_AttractionId_Language",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                columns: new[] { "AttractionId", "Language" },
                unique: true,
                filter: "[AttractionId] IS NOT NULL AND [Language] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_CreatedAt",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_CreatedBy",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_LastModifiedAt",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_LastModifiedBy",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionLocalizedContents_Name",
                schema: "rafeeq",
                table: "AttractionLocalizedContents",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_CreatedAt",
                schema: "rafeeq",
                table: "Attractions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_CreatedBy",
                schema: "rafeeq",
                table: "Attractions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_IsFeatured",
                schema: "rafeeq",
                table: "Attractions",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_LastModifiedAt",
                schema: "rafeeq",
                table: "Attractions",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_LastModifiedBy",
                schema: "rafeeq",
                table: "Attractions",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_SiteId",
                schema: "rafeeq",
                table: "Attractions",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_Type",
                schema: "rafeeq",
                table: "Attractions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CreatedAt",
                schema: "rafeeq",
                table: "Cities",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CreatedBy",
                schema: "rafeeq",
                table: "Cities",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_DisplayOrder",
                schema: "rafeeq",
                table: "Cities",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_LastModifiedAt",
                schema: "rafeeq",
                table: "Cities",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_LastModifiedBy",
                schema: "rafeeq",
                table: "Cities",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StoredFileId",
                schema: "rafeeq",
                table: "Cities",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CityLocalizedContents_CityId",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_CityLocalizedContents_CityId_Language",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                columns: new[] { "CityId", "Language" },
                unique: true,
                filter: "[CityId] IS NOT NULL AND [Language] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CityLocalizedContents_CreatedAt",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CityLocalizedContents_CreatedBy",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CityLocalizedContents_LastModifiedAt",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CityLocalizedContents_LastModifiedBy",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CityLocalizedContents_Name",
                schema: "rafeeq",
                table: "CityLocalizedContents",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ContentId",
                schema: "rafeeq",
                table: "ContentReports",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_CreatedAt",
                schema: "rafeeq",
                table: "ContentReports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_CreatedBy",
                schema: "rafeeq",
                table: "ContentReports",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_LastModifiedAt",
                schema: "rafeeq",
                table: "ContentReports",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_LastModifiedBy",
                schema: "rafeeq",
                table: "ContentReports",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_Priority",
                schema: "rafeeq",
                table: "ContentReports",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ReportedAt",
                schema: "rafeeq",
                table: "ContentReports",
                column: "ReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ReportedBy",
                schema: "rafeeq",
                table: "ContentReports",
                column: "ReportedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_Status",
                schema: "rafeeq",
                table: "ContentReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_CreatedAt",
                schema: "rafeeq",
                table: "Favourites",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_CreatedBy",
                schema: "rafeeq",
                table: "Favourites",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_LastModifiedAt",
                schema: "rafeeq",
                table: "Favourites",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_LastModifiedBy",
                schema: "rafeeq",
                table: "Favourites",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_SiteId",
                schema: "rafeeq",
                table: "Favourites",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_TouristId_SiteId",
                schema: "rafeeq",
                table: "Favourites",
                columns: new[] { "TouristId", "SiteId" },
                unique: true,
                filter: "[TouristId] IS NOT NULL AND [SiteId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportationLocalizedContents_CreatedAt",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportationLocalizedContents_CreatedBy",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportationLocalizedContents_LastModifiedAt",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportationLocalizedContents_LastModifiedBy",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportationLocalizedContents_Name",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportationLocalizedContents_TransportationId",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                column: "TransportationId");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportationLocalizedContents_TransportationId_Language",
                schema: "rafeeq",
                table: "NearestTransportationLocalizedContents",
                columns: new[] { "TransportationId", "Language" },
                unique: true,
                filter: "[TransportationId] IS NOT NULL AND [Language] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportations_CreatedAt",
                schema: "rafeeq",
                table: "NearestTransportations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportations_CreatedBy",
                schema: "rafeeq",
                table: "NearestTransportations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportations_LastModifiedAt",
                schema: "rafeeq",
                table: "NearestTransportations",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportations_LastModifiedBy",
                schema: "rafeeq",
                table: "NearestTransportations",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NearestTransportations_SiteId_Type",
                schema: "rafeeq",
                table: "NearestTransportations",
                columns: new[] { "SiteId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocalizedContents_CreatedAt",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocalizedContents_CreatedBy",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocalizedContents_LastModifiedAt",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocalizedContents_LastModifiedBy",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocalizedContents_OfferId",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocalizedContents_OfferId_Language",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                columns: new[] { "OfferId", "Language" },
                unique: true,
                filter: "[OfferId] IS NOT NULL AND [Language] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocalizedContents_Title",
                schema: "rafeeq",
                table: "OfferLocalizedContents",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CreatedAt",
                schema: "rafeeq",
                table: "Offers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CreatedBy",
                schema: "rafeeq",
                table: "Offers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_LastModifiedAt",
                schema: "rafeeq",
                table: "Offers",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_LastModifiedBy",
                schema: "rafeeq",
                table: "Offers",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_PromoCode",
                schema: "rafeeq",
                table: "Offers",
                column: "PromoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_SponsorId_IsActive",
                schema: "rafeeq",
                table: "Offers",
                columns: new[] { "SponsorId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ValidityPeriod_StartDate_EndDate",
                schema: "rafeeq",
                table: "Offers",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                schema: "rafeeq",
                table: "RefreshTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "rafeeq",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "rafeeq",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CreatedAt",
                schema: "rafeeq",
                table: "Reviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CreatedBy",
                schema: "rafeeq",
                table: "Reviews",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_LastModifiedAt",
                schema: "rafeeq",
                table: "Reviews",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_LastModifiedBy",
                schema: "rafeeq",
                table: "Reviews",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Rating",
                schema: "rafeeq",
                table: "Reviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SiteId",
                schema: "rafeeq",
                table: "Reviews",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SiteId_Status",
                schema: "rafeeq",
                table: "Reviews",
                columns: new[] { "SiteId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Status",
                schema: "rafeeq",
                table: "Reviews",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TouristId",
                schema: "rafeeq",
                table: "Reviews",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "rafeeq",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "rafeeq",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_OpeningHours_SiteId",
                schema: "rafeeq",
                table: "Site_OpeningHours",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_OpeningHours_SiteId_DayOfWeek",
                schema: "rafeeq",
                table: "Site_OpeningHours",
                columns: new[] { "SiteId", "Day" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteImages_CreatedAt",
                schema: "rafeeq",
                table: "SiteImages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SiteImages_CreatedBy",
                schema: "rafeeq",
                table: "SiteImages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SiteImages_LastModifiedAt",
                schema: "rafeeq",
                table: "SiteImages",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SiteImages_LastModifiedBy",
                schema: "rafeeq",
                table: "SiteImages",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SiteImages_SiteId_DisplayOrder",
                schema: "rafeeq",
                table: "SiteImages",
                columns: new[] { "SiteId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SiteImages_SiteId_IsMain",
                schema: "rafeeq",
                table: "SiteImages",
                columns: new[] { "SiteId", "IsMain" });

            migrationBuilder.CreateIndex(
                name: "IX_SiteImages_StoredFileId",
                schema: "rafeeq",
                table: "SiteImages",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_CreatedAt",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_CreatedBy",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_Language_Name",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                columns: new[] { "Language", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_LastModifiedAt",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_LastModifiedBy",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_Name",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_SiteId",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteLocalizedContents_SiteId_Language",
                schema: "rafeeq",
                table: "SiteLocalizedContents",
                columns: new[] { "SiteId", "Language" },
                unique: true,
                filter: "[SiteId] IS NOT NULL AND [Language] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_AverageRating",
                schema: "rafeeq",
                table: "Sites",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_CityId",
                schema: "rafeeq",
                table: "Sites",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_CreatedAt",
                schema: "rafeeq",
                table: "Sites",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_CreatedBy",
                schema: "rafeeq",
                table: "Sites",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_IsFeatured",
                schema: "rafeeq",
                table: "Sites",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_IsHiddenGem",
                schema: "rafeeq",
                table: "Sites",
                column: "IsHiddenGem");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_IsPopular",
                schema: "rafeeq",
                table: "Sites",
                column: "IsPopular");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_LastModifiedAt",
                schema: "rafeeq",
                table: "Sites",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_LastModifiedBy",
                schema: "rafeeq",
                table: "Sites",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Location_Latitude_Longitude",
                schema: "rafeeq",
                table: "Sites",
                columns: new[] { "Location_Latitude", "Location_Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Status",
                schema: "rafeeq",
                table: "Sites",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Type",
                schema: "rafeeq",
                table: "Sites",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorImages_CreatedAt",
                schema: "rafeeq",
                table: "SponsorImages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorImages_CreatedBy",
                schema: "rafeeq",
                table: "SponsorImages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorImages_LastModifiedAt",
                schema: "rafeeq",
                table: "SponsorImages",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorImages_LastModifiedBy",
                schema: "rafeeq",
                table: "SponsorImages",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorImages_SponsorId_DisplayOrder",
                schema: "rafeeq",
                table: "SponsorImages",
                columns: new[] { "SponsorId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SponsorImages_SponsorId_IsMain",
                schema: "rafeeq",
                table: "SponsorImages",
                columns: new[] { "SponsorId", "IsMain" });

            migrationBuilder.CreateIndex(
                name: "IX_SponsorImages_StoredFileId",
                schema: "rafeeq",
                table: "SponsorImages",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_CreatedAt",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_CreatedBy",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_Language_Title",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                columns: new[] { "Language", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_LastModifiedAt",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_LastModifiedBy",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_SponsorId",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_SponsorId_Language",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                columns: new[] { "SponsorId", "Language" },
                unique: true,
                filter: "[SponsorId] IS NOT NULL AND [Language] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorLocalizedContents_Title",
                schema: "rafeeq",
                table: "SponsorLocalizedContents",
                column: "Title",
                unique: true,
                filter: "[Title] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_ContactEmail",
                schema: "rafeeq",
                table: "Sponsors",
                column: "ContactEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_ContractDate_EndDate",
                schema: "rafeeq",
                table: "Sponsors",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_CreatedAt",
                schema: "rafeeq",
                table: "Sponsors",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_CreatedBy",
                schema: "rafeeq",
                table: "Sponsors",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_LastModifiedAt",
                schema: "rafeeq",
                table: "Sponsors",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_LastModifiedBy",
                schema: "rafeeq",
                table: "Sponsors",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_Location_Latitude_Longitude",
                schema: "rafeeq",
                table: "Sponsors",
                columns: new[] { "Location_Latitude", "Location_Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_Status",
                schema: "rafeeq",
                table: "Sponsors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_Tier",
                schema: "rafeeq",
                table: "Sponsors",
                column: "Tier");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_Type",
                schema: "rafeeq",
                table: "Sponsors",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_Hash",
                schema: "rafeeq",
                table: "StoredFiles",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Tourists_CreatedAt",
                schema: "rafeeq",
                table: "Tourists",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tourists_Status",
                schema: "rafeeq",
                table: "Tourists",
                column: "Status");

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
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartDate",
                schema: "rafeeq",
                table: "Trips",
                column: "StartDate");

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
                name: "IX_Trips_UserPosition_LatLng",
                schema: "rafeeq",
                table: "Trips",
                columns: new[] { "UserPosition_Latitude", "UserPosition_Longitude" });

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
                name: "IX_TripSites_SiteLocation_LatLng",
                schema: "rafeeq",
                table: "TripSites",
                columns: new[] { "SiteLocation_Latitude", "SiteLocation_Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_TripId_DisplayOrder",
                schema: "rafeeq",
                table: "TripSites",
                columns: new[] { "TripDayId", "VisitOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TripSites_TripId_PlannedArrivalTime",
                schema: "rafeeq",
                table: "TripSites",
                columns: new[] { "TripDayId", "PlannedArrivalTime" });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "rafeeq",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "rafeeq",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "rafeeq",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "rafeeq",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "rafeeq",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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
                name: "ArtifactImages",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "ArtifactLocalizedContents",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "AttractionImages",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "AttractionLocalizedContents",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "CityLocalizedContents",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "ContentReports",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Favourites",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "NearestTransportationLocalizedContents",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "OfferLocalizedContents",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Site_OpeningHours",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "SiteImages",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "SiteLocalizedContents",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "SponsorImages",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "SponsorLocalizedContents",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "TripSites",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "VisitedSites",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Artifacts",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Attractions",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "NearestTransportations",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Offers",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "TripDay",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Tourists",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Sites",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Sponsors",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Trips",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "rafeeq");

            migrationBuilder.DropTable(
                name: "StoredFiles",
                schema: "rafeeq");
        }
    }
}
