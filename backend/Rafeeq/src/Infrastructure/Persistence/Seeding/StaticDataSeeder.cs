using Domain.Entities.AttractionAggregate;
using Domain.Entities.CityAggregate;
using Domain.Entities.ContentReportAggregate;
using Domain.Entities.ReviewAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Entities.SponsorAggregate;
using Domain.Entities.TouristAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Identity;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Models;

namespace Infrastructure.Persistence.Seeding;

internal static class StaticDataSeeder
{
    private const string TouristEmail = "tourist@rafeeq.local";
    private const string AdminEmail = "admin@rafeeq.local";
    private const string SeedPassword = "Abc@1234";

    private static readonly Guid TouristUserId = Guid.Parse("0A1A2898-9868-4E57-BB75-65F53A4A04A9");
    private static readonly Guid AdminUserId = Guid.Parse("041B2167-9223-4D13-9B2C-C065854BEBDB");

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        await EnsureRolesAsync(roleManager);
        await EnsureUserAsync(userManager, TouristUserId, "swagger-tourist", TouristEmail, SeedPassword, UserRole.Tourist);
        await EnsureUserAsync(userManager, AdminUserId, "swagger-admin", AdminEmail, SeedPassword, UserRole.Admin);

        Tourist tourist = await EnsureTouristAsync(dbContext, TouristUserId, cancellationToken);

        if (await dbContext.Cities.AnyAsync(cancellationToken))
        {
            return;
        }

        City cairo = Ensure(City.Create(
            "Cairo",
            "Egypt's capital with iconic historical landmarks.",
            Ensure(GeoLocation.Create(30.0444, 31.2357), "Create Cairo center location")),
            "Create Cairo city");
        Ensure(cairo.SetImage("https://images.unsplash.com/photo-1539650116574-75c0c6d73d4e"), "Set Cairo image");
        Ensure(cairo.SetDisplayOrder(1), "Set Cairo display order");
        Ensure(cairo.AddLocalizedContent(LanguageCode.English, "Cairo", "Capital city of Egypt."), "Add Cairo localized content");

        City alexandria = Ensure(City.Create(
            "Alexandria",
            "Mediterranean coastal city known for heritage and sea views.",
            Ensure(GeoLocation.Create(31.2001, 29.9187), "Create Alexandria center location")),
            "Create Alexandria city");
        Ensure(alexandria.SetImage("https://images.unsplash.com/photo-1568322445389-f64ac2515021"), "Set Alexandria image");
        Ensure(alexandria.SetDisplayOrder(2), "Set Alexandria display order");
        Ensure(alexandria.AddLocalizedContent(LanguageCode.English, "Alexandria", "Historic port city on the Mediterranean."), "Add Alexandria localized content");

        Site pyramidsSite = Ensure(Site.Create(
            cairo.Id,
            "Giza Plateau",
            "Home of the Great Pyramids and the Sphinx.",
            Ensure(GeoLocation.Create(29.9792, 31.1342), "Create Giza location"),
            Ensure(Address.Create("Al Haram", "Giza", "Giza Governorate", "12556"), "Create Giza address"),
            SiteType.Historical),
            "Create Giza site");
        pyramidsSite.SetAsFeatured(true);
        Ensure(pyramidsSite.SetContactInfo("+201000000001", "https://example.com/giza"), "Set Giza contact");
        pyramidsSite.SetEntryFee(Ensure(Money.Create(200, "EGP"), "Create Giza entry fee"));
        Ensure(pyramidsSite.AddImage("https://images.unsplash.com/photo-1598970434795-0c54fe7c0642", true, 0, "Great Pyramid"), "Add Giza image");
        Ensure(pyramidsSite.AddFacility("Parking", "Large public parking area."), "Add Giza facility");
        Ensure(pyramidsSite.AddLocalizedContent(LanguageCode.English, "Giza Plateau", "Ancient monuments complex."), "Add Giza localized content");
        Ensure(pyramidsSite.AddOpeningHours(
            DayOfWeek.Monday,
            Ensure(TimeRange.Create(new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0)), "Create Giza opening hours"),
            false),
            "Add Giza opening hours");

        Site citadelSite = Ensure(Site.Create(
            cairo.Id,
            "Citadel of Saladin",
            "Historic Islamic-era fortification with city views.",
            Ensure(GeoLocation.Create(30.0287, 31.2619), "Create Citadel location"),
            Ensure(Address.Create("Salah Salem", "Cairo", "Cairo Governorate", "11511"), "Create Citadel address"),
            SiteType.Religious),
            "Create Citadel site");
        Ensure(citadelSite.SetContactInfo("+201000000002", "https://example.com/citadel"), "Set Citadel contact");
        Ensure(citadelSite.AddImage("https://images.unsplash.com/photo-1548013146-72479768bada", true, 0, "Citadel exterior"), "Add Citadel image");
        Ensure(citadelSite.AddLocalizedContent(LanguageCode.English, "Citadel", "Medieval Islamic citadel in Cairo."), "Add Citadel localized content");

        cairo.IncrementSiteCount();
        cairo.IncrementSiteCount();

        Attraction sphinx = Ensure(Attraction.Create(
            pyramidsSite.Id,
            "Great Sphinx",
            "Limestone statue with a lion body and human head.",
            AttractionType.Statue,
            HistoricalPeriod.OldKingdom),
            "Create Sphinx attraction");
        sphinx.SetLocation(Ensure(GeoLocation.Create(29.9753, 31.1376), "Create Sphinx location"), "Near the pyramids entrance");
        Ensure(sphinx.AddImage("https://images.unsplash.com/photo-1524492412937-b28074a5d7da", true, 0, "Sphinx view"), "Add Sphinx image");
        Ensure(sphinx.AddLocalizedContent(LanguageCode.English, "Great Sphinx", "Iconic monument on the Giza plateau."), "Add Sphinx localized content");

        Sponsor sponsor = Ensure(Sponsor.Create(
            "Nile View Restaurant",
            "Popular local restaurant with traveler discounts.",
            SponsorType.Restaurant,
            SponsorTier.Gold,
            Ensure(GeoLocation.Create(30.0426, 31.2389), "Create sponsor location"),
            Ensure(Address.Create("Corniche El Nil", "Cairo", "Cairo Governorate", "11519"), "Create sponsor address"),
            DateTime.UtcNow.Date.AddDays(-15),
            DateTime.UtcNow.Date.AddMonths(6)),
            "Create sponsor");
        Ensure(sponsor.Activate(), "Activate sponsor");
        Ensure(sponsor.AddImage("https://images.unsplash.com/photo-1517248135467-4c7edcad34c4", true, 0, "Restaurant hall"), "Add sponsor image");
        Ensure(sponsor.AddOffer(
            "Tourist Combo",
            "Special meal combo for Rafeeq users.",
            Ensure(Money.Create(150, "EGP"), "Create offer discount"),
            null,
            Ensure(DateRange.Create(DateTime.UtcNow.Date.AddDays(-5), DateTime.UtcNow.Date.AddDays(45)), "Create offer date range"),
            "Valid for dine-in only",
            250),
            "Add sponsor offer");

        Review review = Ensure(Review.Create(
            tourist.Id,
            pyramidsSite.Id,
            Ensure(Rating.Create(5), "Create review rating"),
            "Amazing experience",
            "The place is very organized and breathtaking."),
            "Create review");
        Ensure(review.Approve(), "Approve review");
        review.MarkAsHelpful();
        pyramidsSite.AddRating(Ensure(Rating.Create(5), "Create rating for site aggregate"));
        tourist.IncrementReviewCount();

        ContentReport report = Ensure(ContentReport.Create(
            tourist.Id,
            review.Id,
            ReportReason.Spam,
            "Possible duplicate review content."),
            "Create content report");

        Ensure(tourist.AddFavorite(citadelSite.Id), "Add favorite site");

        dbContext.Cities.AddRange(cairo, alexandria);
        dbContext.Sites.AddRange(pyramidsSite, citadelSite);
        dbContext.Attractions.Add(sphinx);
        dbContext.Sponsors.Add(sponsor);
        dbContext.Reviews.Add(review);
        dbContext.ContentReports.Add(report);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Tourist> EnsureTouristAsync(ApplicationDbContext dbContext, Guid touristId, CancellationToken cancellationToken)
    {
        Tourist? existingTourist = await dbContext.Tourists.FirstOrDefaultAsync(t => t.Id == touristId, cancellationToken);
        if (existingTourist is not null)
        {
            return existingTourist;
        }

        Tourist tourist = Ensure(Tourist.Create(
            touristId,
            "Swagger",
            "Tourist",
            "Egyptian",
            LanguageCode.English),
            "Create tourist profile");

        await dbContext.Tourists.AddAsync(tourist, cancellationToken);
        // await dbContext.SaveChangesAsync(cancellationToken);

        return tourist;
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = [nameof(UserRole.Tourist), nameof(UserRole.Admin), nameof(UserRole.Moderator)];

        foreach (string roleName in roles)
        {
            bool exists = await roleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                IdentityResult createRoleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                Ensure(createRoleResult, $"Create role {roleName}");
            }
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        Guid userId,
        string userName,
        string email,
        string password,
        UserRole role)
    {
        ApplicationUser? existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            bool hasRole = await userManager.IsInRoleAsync(existingUser, role.ToString());
            if (!hasRole)
            {
                IdentityResult addRoleResult = await userManager.AddToRoleAsync(existingUser, role.ToString());
                Ensure(addRoleResult, $"Assign role {role} to {email}");
            }

            return;
        }

        ApplicationUser user = Ensure(ApplicationUser.Create(userId, userName, email), $"Create user object for {email}");
        user.EmailConfirmed = true;

        IdentityResult createUserResult = await userManager.CreateAsync(user, password);
        Ensure(createUserResult, $"Create user {email}");

        IdentityResult assignRoleResult = await userManager.AddToRoleAsync(user, role.ToString());
        Ensure(assignRoleResult, $"Assign role {role} to {email}");
    }

    private static T Ensure<T>(Result<T> result, string operation)
    {
        if (result.Failed)
        {
            throw new InvalidOperationException($"{operation} failed: {result.Error.Code} - {result.Error.Message}");
        }

        return result.Value;
    }

    private static Result Ensure(Result result, string operation)
    {
        if (result.Failed)
        {
            throw new InvalidOperationException($"{operation} failed: {result.Error.Code} - {result.Error.Message}");
        }

        return result;
    }

    private static void Ensure(IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        string details = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
        throw new InvalidOperationException($"{operation} failed: {details}");
    }
}
