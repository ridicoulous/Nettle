using CommandLine;
using CommandLine.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DotSpatial.Projections;
using Microsoft.Data.Sqlite;
using System.Globalization;
namespace Nettle
{
    internal class Program
    {
        static ProjectionInfo wsg = KnownCoordinateSystems.Geographic.World.WGS1984;
        static ProjectionInfo sourceProjection;
        static string tablequery = @"CREATE TABLE IF NOT EXISTS Data (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT,
    time TEXT,
    description TEXT,
    lat REAL,
    lon REAL);";
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                      .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
                      .WithNotParsed<Options>((errs) => HandleParseError(errs));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Shit happens: {ex.ToString()}");
            }
        }

        static void RunOptionsAndReturnExitCode(Options opts)
        {
            sourceProjection = ProjectionInfo.FromEpsgCode(opts.EspgId);
            Console.WriteLine($"Try to read {opts.InputFile}");
            var records = ReadCsvFile(opts);
            Console.WriteLine($"Projecting coordinates from readed file");
            InsertDataIntoDatabase(records, opts.SqliteDbOutputPath);
        }

        static IEnumerable<CsvRecord> ReadCsvFile(Options opts)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = opts.Delimeter
            };
            using (var reader = new StreamReader(opts.InputFile))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<CsvRecordMap>();
                var records = csv.GetRecords<CsvRecord>().ToList();
                return records.Select(record =>
                {
                    var coords = record.Coords.Split(' ').Select(part =>
                    {
                        var sanitizedPart = part.Replace("-", string.Empty);
                        return double.Parse(sanitizedPart);
                    }).ToArray();
                    record.LatLon = coords;
                    if (opts.ShouldSwapXandY)
                        Array.Reverse(record.LatLon);

                    Console.Write($"EPSG:{opts.EspgId} [{record.LatLon[0]},{record.LatLon[1]}] => WSG84 ");
                    Reproject.ReprojectPoints(record.LatLon, null, sourceProjection, wsg, 0, 1);
                    
                    Console.WriteLine($"[{record.LatLon[0]},{record.LatLon[1]}]");
                    return record;
                }).ToList();
            }
        }

        static void InsertDataIntoDatabase(IEnumerable<CsvRecord> records, string databaseFilePath)
        {
            Console.WriteLine($"Inserting readed and projected {records.Count()} to {databaseFilePath}");

            var connectionString = $"Data Source={databaseFilePath}";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var table = connection.CreateCommand();
                table.CommandText = tablequery;
                table.ExecuteNonQuery();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var record in records)
                    {
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = @"INSERT INTO Data (name, time, description, lat, lon) VALUES (@name, @time, @description, @lat, @lon)";
                        cmd.Parameters.AddWithValue("@name", record.Name);
                        cmd.Parameters.AddWithValue("@time", record.Time);
                        cmd.Parameters.AddWithValue("@description", record.Description);
                        cmd.Parameters.AddWithValue("@lat", record.LatLon[0]);
                        cmd.Parameters.AddWithValue("@lon", record.LatLon[1]);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Error parsing command line arguments");
            foreach (var err in errs)
            {
                Console.WriteLine(err.ToString());
            }
        }
    }

}

