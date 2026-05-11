using Application.Commands.Artifacts;
using Application.Commands.Attractions;
using Application.Commands.Cities;
using Application.Commands.Sites;
using Application.Commands.Sites.NearestTransportations;
using Application.Commands.Sites.OpeningHours;
using CsvHelper.Configuration;

namespace Infrastructure.Persistence.Seeding;

internal sealed class CsvMapRegistry
{
    private readonly Dictionary<Type, Type> _maps = new()
    {
        { typeof(SiteCsvRowDto), typeof(SitesCsvRowMap) },
        { typeof(CityCsvRowDto), typeof(CitiesCsvRowMap) },
        { typeof(AttractionCsvRowDto), typeof(AttractionsCsvRowMap) },
        { typeof(ArtifactCsvRowDto), typeof(ArtifactsCsvRowMap) },
        { typeof(NearestTransportationCsvRowDto), typeof(NearestTransportationCsvRowMap) },
        { typeof(OpeningHourCsvRowDto), typeof(OpeningHourCsvRowMap) },
    };

    public Type? GetMapFor<T>()
    {
        return _maps.TryGetValue(typeof(T), out var map)
            ? map
            : null;
    }
}

internal sealed class CitiesCsvRowMap : ClassMap<CityCsvRowDto>
{
    public CitiesCsvRowMap()
    {
        Map(m => m.CityId).Name("City ID");
        Map(m => m.NameEn).Name("City Name (English)");
        Map(m => m.NameAr).Name("City Name (Localized)");
        Map(m => m.DescriptionEn).Name("Description (English)");
        Map(m => m.DescriptionAr).Name("Description (Localized)");
        Map(m => m.Latitude).Name("Center Latitude");
        Map(m => m.Longitude).Name("Center Longitude");
        Map(m => m.DisplayOrder).Name("Display Order");
    }
}

internal sealed class SitesCsvRowMap : ClassMap<SiteCsvRowDto>
{
    public SitesCsvRowMap()
    {
        Map(m => m.SiteId).Name("Site ID");
        Map(m => m.CityName).Name("City Name (English)");
        Map(m => m.NameEn).Name("Name (English)");
        Map(m => m.NameAr).Name("Name (Localized)");
        Map(m => m.DescriptionEn).Name("Description (English)");
        Map(m => m.DescriptionAr).Name("Description (Localized)");
        Map(m => m.AddressEn).Name("Address (English)");
        Map(m => m.AddressAr).Name("Address (Localized)");
        Map(m => m.Status).Name("Status");
        Map(m => m.Type).Name("Type");
        Map(m => m.Latitude).Name("Latitude");
        Map(m => m.Longitude).Name("Longitude");
        Map(m => m.EntryFee).Name("Entry Fee 'NULLABLE'");
        Map(m => m.EntryFeeNoteEn).Name("Entry Fee Note (English) 'NULLABLE'");
        Map(m => m.EntryFeeNoteAr).Name("Entry Fee Note (Localized) 'NULLABLE'");
        Map(m => m.IsFree).Name("Is Free?");
        Map(m => m.EstimatedDurationMinutes).Name("Estimated Duration Minutes");
        Map(m => m.WebsiteUrl).Name("Website URL 'NULLABLE'");
        Map(m => m.ContactPhone).Name("Contact Phone 'NULLABLE'");
        Map(m => m.IsFeatured).Name("Is Featured?");
        Map(m => m.IsHiddenGem).Name("Is Hidden Gem?");
        Map(m => m.Facilities).Name("Facilities (comma-separated)");
    }
}

internal sealed class AttractionsCsvRowMap : ClassMap<AttractionCsvRowDto>
{
    public AttractionsCsvRowMap()
    {
        Map(m => m.AttractionId).Name("Attraction ID");
        Map(m => m.SiteName).Name("Site Name (English)");
        Map(m => m.NameEn).Name("Name (English)");
        Map(m => m.NameAr).Name("Name (Localized)");
        Map(m => m.DescriptionEn).Name("Description (English)");
        Map(m => m.DescriptionAr).Name("Description (Localized)");
        Map(m => m.LocationGuidEn).Name("Location GUID (English)");
        Map(m => m.LocationGuidAr).Name("Location GUID (Localized)");
        Map(m => m.Type).Name("Type");
        Map(m => m.Latitude).Name("Latitude");
        Map(m => m.Longitude).Name("Longitude");
        Map(m => m.IsFeatured).Name("Is Featured?");
        Map(m => m.HistoricalPeriods).Name("Historical Periods (comma-separated)");
    }
}

internal sealed class ArtifactsCsvRowMap : ClassMap<ArtifactCsvRowDto>
{
    public ArtifactsCsvRowMap()
    {
        Map(m => m.ArtifactId).Name("Artifact ID");
        Map(m => m.SiteName).Name("Site Name");
        Map(m => m.NameEn).Name("Name");
        Map(m => m.NameAr).Name("Name (Arabic)");
        Map(m => m.DescriptionEn).Name("Description");
        Map(m => m.DescriptionAr).Name("Description (Arabic)");
        Map(m => m.Type).Name("Type");
        Map(m => m.DisplayOrder).Name("Display Order");
    }
}

internal sealed class NearestTransportationCsvRowMap : ClassMap<NearestTransportationCsvRowDto>
{
    public NearestTransportationCsvRowMap()
    {
        Map(m => m.TransportId).Name("Transport ID");
        Map(m => m.SiteName).Name("Site Name");
        Map(m => m.NameEn).Name("Transport Name (English)");
        Map(m => m.NameAr).Name("Transport Name (Localized)");
        Map(m => m.DescriptionEn).Name("Description (English)");
        Map(m => m.DescriptionAr).Name("Description (Localized)");
        Map(m => m.AddressEn).Name("Address / Location Info (English)");
        Map(m => m.AddressAr).Name("Address / Location Info (Localized)");
        Map(m => m.Type).Name("Transport Type");
        Map(m => m.Latitude).Name("Latitude");
        Map(m => m.Longitude).Name("Longitude");
        Map(m => m.DistanceKm).Name("Distance to Site (Km)");
        Map(m => m.IsOperational).Name("Is Operational?");
        Map(m => m.HasAccessibility).Name("Has Accessibility?");
        Map(m => m.OperatingHours).Name("Operating Hours");
    }
}

internal sealed class OpeningHourCsvRowMap : ClassMap<OpeningHourCsvRowDto>
{
    public OpeningHourCsvRowMap()
    {
        Map(m => m.OpeningHourId).Name("Opening Hour ID");
        Map(m => m.ParentSiteId).Name("Parent Site ID");
        Map(m => m.SiteName).Name("Site Name");
        Map(m => m.Day).Name("Day");
        Map(m => m.StartTime).Name("Start Time");
        Map(m => m.EndTime).Name("End Time");
        Map(m => m.IsOvernight).Name("Is Overnight?");
        Map(m => m.IsClosed).Name("Is Closed?");
    }
}