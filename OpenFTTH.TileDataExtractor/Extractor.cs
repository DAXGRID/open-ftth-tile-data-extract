using OpenFTTH.TileDataExtractor.GeoJson;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFTTH.TileDataExtractor
{
    public class Extractor
    {
        private readonly string _postgresConnectionString;

        public Extractor(string postgresConnectionString)
        {
            _postgresConnectionString = postgresConnectionString;
        }

        public void Run(string outputFileName)
        {
            Counter counter = new();

            var layers = new List<ILayer>
            {
                new RouteNodeLayer(_postgresConnectionString, counter),
                new RouteSegmentLayer(_postgresConnectionString, counter)
            };

            var writer = new GeoJsonWriter(outputFileName);

            foreach (var layer in layers)
            {
                Log.Information($"Start processing: {layer.Name}...");
                var layerObjects = layer.GetObjects();
                writer.Write(layerObjects);
                Log.Information($"{layerObjects.Count} {layer.Name} objects written to geojson file.");
            }
        }
    }
}
