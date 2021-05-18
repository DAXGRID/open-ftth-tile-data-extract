using Serilog;

namespace OpenFTTH.TileDataExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogging();

            if (args.Length != 2)
            {
                Log.Error("Usage: postgres-connection-string output.geojson");
            }

            var postgresConnectionString = args[0];
            var outputFilename = args[1];

            var extractor = new Extractor(postgresConnectionString);

            extractor.Run(outputFilename);

            Log.Information("Finish.");
        }


        static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .CreateLogger();
        }
    }
}
