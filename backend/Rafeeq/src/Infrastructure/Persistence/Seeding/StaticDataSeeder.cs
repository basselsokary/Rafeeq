using Domain.Common.Constants;
using Domain.Entities.AttractionAggregate;
using Domain.Entities.CityAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Entities.SponsorAggregate;
using Domain.Entities.TouristAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Identity.Entities;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;

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
        
        // await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Tourists.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await EnsureRolesAsync(roleManager);
        await EnsureUserAsync(userManager, TouristUserId, "swagger-tourist", TouristEmail, SeedPassword, UserRoles.Tourist);
        await EnsureUserAsync(userManager, AdminUserId, "swagger-admin", AdminEmail, SeedPassword, UserRoles.Admin);

        Tourist tourist = await EnsureTouristAsync(dbContext, TouristUserId, cancellationToken);

        var cities = new List<City>
        {
            Ensure(City.Create(
                "Cairo",
                "Egypt's capital and cultural hub.",
                Ensure(GeoLocation.Create(30.0444, 31.2357), "Create Cairo location"),
                1),
                "Create Cairo city"),
            Ensure(City.Create(
                "Alexandria",
                "Mediterranean coastal governorate.",
                Ensure(GeoLocation.Create(31.2001, 29.9187), "Create Alexandria location"),
                2),
                "Create Alexandria city"),
            Ensure(City.Create(
                "Giza",
                "Gateway to the western plateau.",
                Ensure(GeoLocation.Create(30.0131, 31.2089), "Create Giza location"),
                3),
                "Create Giza city"),
            Ensure(City.Create(
                "Luxor",
                "Upper Egypt governorate on the Nile.",
                Ensure(GeoLocation.Create(25.6872, 32.6396), "Create Luxor location"),
                4),
                "Create Luxor city"),
            Ensure(City.Create(
                "Aswan",
                "Southern gateway with river islands.",
                Ensure(GeoLocation.Create(24.0889, 32.8998), "Create Aswan location"),
                5),
                "Create Aswan city")
        };

        Ensure(cities[0].AddLocalizedContent(LanguageCode.Arabic, "القاهرة", "عاصمة مصر ومركزها الثقافي."), "Add Cairo Arabic content");
        Ensure(cities[1].AddLocalizedContent(LanguageCode.Arabic, "الإسكندرية", "محافظة ساحلية على البحر المتوسط."), "Add Alexandria Arabic content");
        Ensure(cities[2].AddLocalizedContent(LanguageCode.Arabic, "الجيزة", "بوابة الهضبة الغربية.") , "Add Giza Arabic content");
        Ensure(cities[3].AddLocalizedContent(LanguageCode.Arabic, "الأقصر", "محافظة بصعيد مصر على النيل."), "Add Luxor Arabic content");
        Ensure(cities[4].AddLocalizedContent(LanguageCode.Arabic, "أسوان", "بوابة الجنوب مع جزر نيلية."), "Add Aswan Arabic content");

        var weekdayHours = Ensure(TimeRange.Create(new TimeOnly(9, 0, 0), new TimeOnly(17, 0, 0)), "Create weekday hours");
        var weekendHours = Ensure(TimeRange.Create(new TimeOnly(10, 0, 0), new TimeOnly(18, 0, 0)), "Create weekend hours");

        var siteDefinitions = new[]
        {
            new { City = cities[0], NameEn = "Riverfront Plaza", NameAr = "ساحة الواجهة النهرية", DescEn = "Open plaza with river views.", DescAr = "ساحة مفتوحة بإطلالات على النهر.", AddressEn = "Riverfront Street 1, Cairo", AddressAr = "شارع الواجهة النهرية 1، القاهرة", Lat = 30.0511, Lng = 31.2412, Type = SiteType.Walkway, Duration = 90 },
            new { City = cities[0], NameEn = "Old Market Courtyard", NameAr = "فناء السوق القديم", DescEn = "Historic market with artisan stalls.", DescAr = "سوق تاريخي مع أكشاك حرفية.", AddressEn = "Market Lane 3, Cairo", AddressAr = "حارة السوق 3، القاهرة", Lat = 30.0535, Lng = 31.2360, Type = SiteType.Cultural, Duration = 120 },
            new { City = cities[0], NameEn = "City Museum Annex", NameAr = "ملحق متحف المدينة", DescEn = "Compact museum gallery.", DescAr = "قاعة متحف مصغرة.", AddressEn = "Museum Avenue 2, Cairo", AddressAr = "شارع المتحف 2، القاهرة", Lat = 30.0470, Lng = 31.2320, Type = SiteType.Museum, Duration = 150 },
            new { City = cities[0], NameEn = "Central Garden", NameAr = "الحديقة المركزية", DescEn = "Green park for family strolls.", DescAr = "حديقة خضراء لنزهات العائلة.", AddressEn = "Garden Road 5, Cairo", AddressAr = "طريق الحديقة 5، القاهرة", Lat = 30.0565, Lng = 31.2448, Type = SiteType.Park, Duration = 80 },
            new { City = cities[1], NameEn = "Harbor Walk", NameAr = "ممشى الميناء", DescEn = "Coastal walkway with cafes.", DescAr = "ممشى ساحلي مع مقاهي.", AddressEn = "Harbor Road 8, Alexandria", AddressAr = "طريق الميناء 8، الإسكندرية", Lat = 31.2125, Lng = 29.9022, Type = SiteType.Walkway, Duration = 100 },
            new { City = cities[1], NameEn = "Sea Fort", NameAr = "حصن البحر", DescEn = "Small fortress overlooking the bay.", DescAr = "حصن صغير يطل على الخليج.", AddressEn = "Fort Street 4, Alexandria", AddressAr = "شارع الحصن 4، الإسكندرية", Lat = 31.2148, Lng = 29.8894, Type = SiteType.Fortress, Duration = 140 },
            new { City = cities[1], NameEn = "Coastal Beach", NameAr = "شاطئ الساحل", DescEn = "Public beach with calm waters.", DescAr = "شاطئ عام بمياه هادئة.", AddressEn = "Beach Drive 1, Alexandria", AddressAr = "طريق الشاطئ 1، الإسكندرية", Lat = 31.2080, Lng = 29.9150, Type = SiteType.Beach, Duration = 180 },
            new { City = cities[2], NameEn = "Giza Plateau Park", NameAr = "حديقة هضبة الجيزة", DescEn = "Open park near the plateau.", DescAr = "حديقة مفتوحة قرب الهضبة.", AddressEn = "Plateau Road 2, Giza", AddressAr = "طريق الهضبة 2، الجيزة", Lat = 30.0155, Lng = 31.1310, Type = SiteType.Park, Duration = 110 },
            new { City = cities[2], NameEn = "Westbank Gate", NameAr = "بوابة الضفة الغربية", DescEn = "Gateway to western trails.", DescAr = "بوابة لمسارات الضفة الغربية.", AddressEn = "Westbank Street 6, Giza", AddressAr = "شارع الضفة الغربية 6، الجيزة", Lat = 30.0100, Lng = 31.2050, Type = SiteType.Gate, Duration = 95 },
            new { City = cities[2], NameEn = "Heritage Court", NameAr = "ساحة التراث", DescEn = "Cultural courtyard for events.", DescAr = "ساحة ثقافية للفعاليات.", AddressEn = "Heritage Avenue 7, Giza", AddressAr = "شارع التراث 7، الجيزة", Lat = 30.0185, Lng = 31.2005, Type = SiteType.Cultural, Duration = 120 },
            new { City = cities[3], NameEn = "Temple Walk", NameAr = "ممشى المعبد", DescEn = "Walkway near ancient sites.", DescAr = "ممشى قرب المواقع الأثرية.", AddressEn = "Temple Road 3, Luxor", AddressAr = "طريق المعبد 3، الأقصر", Lat = 25.6990, Lng = 32.6390, Type = SiteType.Walkway, Duration = 90 },
            new { City = cities[3], NameEn = "Luxor Market", NameAr = "سوق الأقصر", DescEn = "Local market for crafts.", DescAr = "سوق محلي للحرف.", AddressEn = "Market Street 5, Luxor", AddressAr = "شارع السوق 5، الأقصر", Lat = 25.6920, Lng = 32.6405, Type = SiteType.Cultural, Duration = 130 },
            new { City = cities[3], NameEn = "Nile Heritage Museum", NameAr = "متحف تراث النيل", DescEn = "Museum about river heritage.", DescAr = "متحف عن تراث النهر.", AddressEn = "Museum Avenue 1, Luxor", AddressAr = "شارع المتحف 1، الأقصر", Lat = 25.6870, Lng = 32.6370, Type = SiteType.Museum, Duration = 160 },
            new { City = cities[4], NameEn = "Island Viewpoint", NameAr = "مطل الجزر", DescEn = "Viewpoint over river islands.", DescAr = "مطل على الجزر النيلية.", AddressEn = "Island Road 4, Aswan", AddressAr = "طريق الجزر 4، أسوان", Lat = 24.0910, Lng = 32.8970, Type = SiteType.Island, Duration = 100 },
            new { City = cities[4], NameEn = "Aswan Riverside", NameAr = "واجهة أسوان", DescEn = "Riverside promenade.", DescAr = "ممشى على ضفاف النهر.", AddressEn = "Riverside Avenue 8, Aswan", AddressAr = "شارع الواجهة 8، أسوان", Lat = 24.0875, Lng = 32.8990, Type = SiteType.Walkway, Duration = 120 },
            new { City = cities[4], NameEn = "Granite Plaza", NameAr = "ساحة الجرانيت", DescEn = "Open plaza with granite exhibits.", DescAr = "ساحة مفتوحة مع معروضات الجرانيت.", AddressEn = "Granite Street 9, Aswan", AddressAr = "شارع الجرانيت 9، أسوان", Lat = 24.0850, Lng = 32.9030, Type = SiteType.Cultural, Duration = 110 }
        };

        var sites = new List<Site>();
        var attractions = new List<Attraction>();

        for (var i = 0; i < siteDefinitions.Length; i++)
        {
            var siteDef = siteDefinitions[i];
            var addressEn = Ensure(Address.Create(siteDef.AddressEn), $"Create {siteDef.NameEn} English address");
            var addressAr = Ensure(Address.Create(siteDef.AddressAr), $"Create {siteDef.NameEn} Arabic address");
            var location = Ensure(GeoLocation.Create(siteDef.Lat, siteDef.Lng), $"Create {siteDef.NameEn} location");

            var site = Ensure(Site.Create(
                siteDef.City.Id,
                siteDef.NameEn,
                siteDef.DescEn,
                addressEn,
                null,
                location,
                siteDef.Type,
                siteDef.Duration,
                $"+2010000000{i + 1:00}",
                $"https://site-{i + 1}.example.com"),
                $"Create {siteDef.NameEn} site");

            Ensure(site.AddLocalizedContent(LanguageCode.Arabic, siteDef.NameAr, siteDef.DescAr, addressAr, null), $"Add {siteDef.NameEn} Arabic content");
            Ensure(site.AddOpeningHour(WeekDay.Monday, weekdayHours, false), $"Add {siteDef.NameEn} Monday hours");
            Ensure(site.AddOpeningHour(WeekDay.Saturday, weekendHours, false), $"Add {siteDef.NameEn} Saturday hours");
            Ensure(site.AddFacilities([FacilityType.Parking, FacilityType.Restrooms, FacilityType.InformationCenter]), $"Add {siteDef.NameEn} facilities");

            var transportA = Ensure(GeoLocation.Create(siteDef.Lat + 0.01, siteDef.Lng + 0.01), $"Create {siteDef.NameEn} transport A location");
            var transportB = Ensure(GeoLocation.Create(siteDef.Lat - 0.01, siteDef.Lng - 0.01), $"Create {siteDef.NameEn} transport B location");
            var busStop = Ensure(site.AddNearestTransportation(TransportationType.Bus, transportA, 0.6), $"Add {siteDef.NameEn} bus stop");
            var metroStop = Ensure(site.AddNearestTransportation(TransportationType.Metro, transportB, 1.2), $"Add {siteDef.NameEn} metro stop");

            var busAddressEn = Ensure(Address.Create($"Bus Stop A, {siteDef.AddressEn}"), $"Create {siteDef.NameEn} bus stop English address");
            var busAddressAr = Ensure(Address.Create($"محطة الحافلات أ، {siteDef.AddressAr}"), $"Create {siteDef.NameEn} bus stop Arabic address");
            Ensure(busStop.AddLocalizedContent(
                LanguageCode.English,
                $"{siteDef.NameEn} Bus Stop",
                $"Primary bus stop near {siteDef.NameEn}.",
                busAddressEn),
                $"Add {siteDef.NameEn} bus stop English content");
            Ensure(busStop.AddLocalizedContent(
                LanguageCode.Arabic,
                $"محطة حافلات {siteDef.NameAr}",
                $"محطة الحافلات الرئيسية قرب {siteDef.NameAr}.",
                busAddressAr),
                $"Add {siteDef.NameEn} bus stop Arabic content");

            var metroAddressEn = Ensure(Address.Create($"Metro Station B, {siteDef.AddressEn}"), $"Create {siteDef.NameEn} metro English address");
            var metroAddressAr = Ensure(Address.Create($"محطة المترو ب، {siteDef.AddressAr}"), $"Create {siteDef.NameEn} metro Arabic address");
            Ensure(metroStop.AddLocalizedContent(
                LanguageCode.English,
                $"{siteDef.NameEn} Metro",
                $"Metro access point for {siteDef.NameEn}.",
                metroAddressEn),
                $"Add {siteDef.NameEn} metro English content");
            Ensure(metroStop.AddLocalizedContent(
                LanguageCode.Arabic,
                $"مترو {siteDef.NameAr}",
                $"نقطة وصول المترو إلى {siteDef.NameAr}.",
                metroAddressAr),
                $"Add {siteDef.NameEn} metro Arabic content");

            Ensure(site.UpdateStatus(SiteStatus.Active, false, false), $"Activate {siteDef.NameEn} site");

            siteDef.City.IncrementSiteCount();
            sites.Add(site);

            for (var j = 1; j <= 3; j++)
            {
                var attraction = Ensure(Attraction.Create(
                    site.Id,
                    $"{siteDef.NameEn} Landmark {j}",
                    $"Highlight {j} near {siteDef.NameEn}.",
                    $"Zone {j}",
                    AttractionType.ViewingPoint,
                    new List<HistoricalPeriod> { HistoricalPeriod.OldKingdom, HistoricalPeriod.NewKingdom }),
                    $"Create {siteDef.NameEn} attraction {j}");

                Ensure(attraction.AddLocalizedContent(
                    LanguageCode.Arabic,
                    $"معلم {siteDef.NameAr} {j}",
                    $"معلم بارز {j} بالقرب من {siteDef.NameAr}.",
                    $"منطقة {j}"),
                    $"Add {siteDef.NameEn} attraction {j} Arabic content");

                attraction.SetLocation(Ensure(
                    GeoLocation.Create(siteDef.Lat + (0.001 * j), siteDef.Lng + (0.001 * j)),
                    $"Create {siteDef.NameEn} attraction {j} location"));

                attractions.Add(attraction);
            }
        }

        var sponsorDefinitions = new[]
        {
            new { NameEn = "Nile Bistro", NameAr = "بيسترو النيل", DescEn = "Riverside dining and offers.", DescAr = "مطعم على النيل مع عروض.", AddressEn = "Corniche Road 10, Nile Capital", AddressAr = "طريق الكورنيش 10، عاصمة النيل", Lat = 30.0520, Lng = 31.2380, Type = SponsorType.Restaurant, Tier = SponsorTier.Gold },
            new { NameEn = "Harbor Inn", NameAr = "نزل الميناء", DescEn = "Boutique stay near the bay.", DescAr = "إقامة مميزة قرب الخليج.", AddressEn = "Harbor Street 2, Coastal Vista", AddressAr = "شارع الميناء 2، واجهة الساحل", Lat = 31.2133, Lng = 29.8933, Type = SponsorType.Hotel, Tier = SponsorTier.Silver },
            new { NameEn = "Desert Trails", NameAr = "مسارات الصحراء", DescEn = "Guided desert experiences.", DescAr = "جولات صحراء موجهة.", AddressEn = "Trail Road 6, Desert Gate", AddressAr = "طريق المسارات 6، بوابة الصحراء", Lat = 29.9845, Lng = 30.9490, Type = SponsorType.Tour, Tier = SponsorTier.Gold },
            new { NameEn = "Coastline Transit", NameAr = "تنقلات الساحل", DescEn = "Local transport services.", DescAr = "خدمات نقل محلية.", AddressEn = "Transit Blvd 1, Coastal Vista", AddressAr = "بوليفارد النقل 1، واجهة الساحل", Lat = 31.2098, Lng = 29.9100, Type = SponsorType.Transportation, Tier = SponsorTier.Silver },
            new { NameEn = "Oasis Spa", NameAr = "سبا الواحة", DescEn = "Wellness and relaxation.", DescAr = "عناية واسترخاء.", AddressEn = "Oasis Road 3, Desert Gate", AddressAr = "طريق الواحة 3، بوابة الصحراء", Lat = 29.9735, Lng = 30.9475, Type = SponsorType.Service, Tier = SponsorTier.Platinum },
            new { NameEn = "Museum Cafe", NameAr = "مقهى المتحف", DescEn = "Cafe for visitors and families.", DescAr = "مقهى للزوار والعائلات.", AddressEn = "Museum Avenue 1, Nile Capital", AddressAr = "شارع المتحف 1، عاصمة النيل", Lat = 30.0465, Lng = 31.2315, Type = SponsorType.Restaurant, Tier = SponsorTier.Gold }
        };

        var sponsors = new List<Sponsor>();

        for (var i = 0; i < sponsorDefinitions.Length; i++)
        {
            var sponsorDef = sponsorDefinitions[i];
            var addressEn = Ensure(Address.Create(sponsorDef.AddressEn), $"Create {sponsorDef.NameEn} English address");
            var addressAr = Ensure(Address.Create(sponsorDef.AddressAr), $"Create {sponsorDef.NameEn} Arabic address");
            var location = Ensure(GeoLocation.Create(sponsorDef.Lat, sponsorDef.Lng), $"Create {sponsorDef.NameEn} location");
            var contract = Ensure(DateRange.Create(DateTime.UtcNow.Date.AddDays(-15), DateTime.UtcNow.Date.AddMonths(6)), $"Create {sponsorDef.NameEn} contract");

            var sponsor = Ensure(Sponsor.Create(
                sponsorDef.NameEn,
                sponsorDef.DescEn,
                addressEn,
                sponsorDef.Type,
                sponsorDef.Tier,
                location,
                contract,
                $"https://sponsor-{i + 1}.example.com",
                null,
                null),
                $"Create {sponsorDef.NameEn} sponsor");

            Ensure(sponsor.AddLocalizedContent(LanguageCode.Arabic, sponsorDef.NameAr, sponsorDef.DescAr, addressAr), $"Add {sponsorDef.NameEn} Arabic content");
            Ensure(sponsor.Activate(), $"Activate {sponsorDef.NameEn} sponsor");

            for (var j = 1; j <= 3; j++)
            {
                var discount = Ensure(Money.Create(100 + (j * 20), "EGP"), $"Create {sponsorDef.NameEn} offer {j} discount");
                var offerRange = Ensure(DateRange.Create(DateTime.UtcNow.Date.AddDays(-2), DateTime.UtcNow.Date.AddDays(30 + (j * 10))), $"Create {sponsorDef.NameEn} offer {j} range");

                var offer = Ensure(sponsor.AddOffer(discount, null, offerRange, 200 + (j * 50), $"PROMO{i + 1}{j}"), $"Add {sponsorDef.NameEn} offer {j}");
                Ensure(offer.Activate(), $"Activate {sponsorDef.NameEn} offer {j}");

                var termsEn = $"Valid for {sponsorDef.NameEn}. One use per customer.";
                var termsAr = $"سارية لدى {sponsorDef.NameAr}. استخدام واحد لكل عميل.";

                Ensure(offer.AddLocalizedContent(
                    LanguageCode.English,
                    $"{sponsorDef.NameEn} Offer {j}",
                    $"Deal {j} for {sponsorDef.NameEn}.",
                    termsEn),
                    $"Add {sponsorDef.NameEn} offer {j} English content");

                Ensure(offer.AddLocalizedContent(
                    LanguageCode.Arabic,
                    $"عرض {sponsorDef.NameAr} {j}",
                    $"عرض {j} لـ {sponsorDef.NameAr}.",
                    termsAr),
                    $"Add {sponsorDef.NameEn} offer {j} Arabic content");
            }

            sponsors.Add(sponsor);
        }

        dbContext.Cities.AddRange(cities);
        dbContext.Sites.AddRange(sites);
        dbContext.Attractions.AddRange(attractions);
        dbContext.Sponsors.AddRange(sponsors);

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
            "Egyptian"),
            "Create tourist profile");

        await dbContext.Tourists.AddAsync(tourist, cancellationToken);
        // await dbContext.SaveChangesAsync(cancellationToken);

        return tourist;
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = UserRoles.AllRoles;

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
        string role)
    {
        ApplicationUser? existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            bool hasRole = await userManager.IsInRoleAsync(existingUser, role);
            if (!hasRole)
            {
                IdentityResult addRoleResult = await userManager.AddToRoleAsync(existingUser, role);
                Ensure(addRoleResult, $"Assign role {role} to {email}");
            }

            return;
        }

        if (role == UserRoles.Admin)
        {
            AdminUser user = Ensure(AdminUser.Create(userId, userName, email, "Sandor", "Clegane", "Sandor The Hound Clegane"), $"Create user object for {email}");
            user.EmailConfirmed = true;

            IdentityResult createUserResult = await userManager.CreateAsync(user, password);
            Ensure(createUserResult, $"Create user {email}");

            IdentityResult assignRoleResult = await userManager.AddToRoleAsync(user, role);
            Ensure(assignRoleResult, $"Assign role {role} to {email}");
        } 
        else if (role == UserRoles.Tourist)
        {
            TouristUser user = Ensure(TouristUser.Create(userId, userName, email), $"Create user object for {email}");
            user.EmailConfirmed = true;

            IdentityResult createUserResult = await userManager.CreateAsync(user, password);
            Ensure(createUserResult, $"Create user {email}");

            IdentityResult assignRoleResult = await userManager.AddToRoleAsync(user, role);
            Ensure(assignRoleResult, $"Assign role {role} to {email}");
        } 
        else
        {
            throw new InvalidOperationException($"Unsupported role: {role}");
        }

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
