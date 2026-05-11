using System.Globalization;
using Application.Common.Interfaces.Services;
using CsvHelper;
using CsvHelper.Configuration;
using Infrastructure.Persistence.Seeding;

namespace Infrastructure.Services;

internal class CsvParser(CsvMapRegistry mapRegistry) : ICsvFileParser
{
    public List<T> ParseCsv<T>(Stream file)
    {
        using var reader = new StreamReader(file);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,

            // Prevent crashes on missing/extra columns
            HeaderValidated = null,
            MissingFieldFound = null,

            // Error handling
            ReadingExceptionOccurred = args =>
            {
                try
                {
                    var context = args.Exception.Context;

                    Console.WriteLine("CSV Conversion Error:");
                    Console.WriteLine($"Message: {args.Exception.Message.Substring(0, Math.Min(200, args.Exception.Message.Length))}"); // Limit message length

                    if (context?.Parser != null)
                    {
                        Console.WriteLine($"Row: {context.Parser.Row}");
                        Console.WriteLine($"Raw: {context.Parser.RawRecord}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while logging CSV exception: {e.Message}");
                }

                return false;
            }
        };

        using var csv = new CsvReader(reader, config);

        // Register map if exists
        var mapType = mapRegistry.GetMapFor<T>();
        if (mapType != null)
        {
            csv.Context.RegisterClassMap(mapType);
        }

        return csv.GetRecords<T>().ToList();
    }
}