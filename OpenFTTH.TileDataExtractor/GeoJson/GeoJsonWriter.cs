using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenFTTH.TileDataExtractor.GeoJson
{
    public class GeoJsonWriter
    {
        private readonly string _fileName;

        private bool _first = true;
     
        public GeoJsonWriter(string fileName)
        {
            _fileName = fileName;
        }

        public void Begin()
        {
            using StreamWriter geoJsonFile = new StreamWriter(_fileName, false, Encoding.UTF8);

            // Start geojson feature collection
            geoJsonFile.WriteLine("{ \"type\": \"FeatureCollection\", \"features\": [");

            geoJsonFile.Close();
        }

        public void Write(IEnumerable<GeoJsonObject> objects)
        {
            using StreamWriter geoJsonFile = new StreamWriter(_fileName, true, Encoding.UTF8);

            foreach (var obj in objects)
            {
                if (!_first)
                    geoJsonFile.Write(",");

                string json = CreateFeatureJsonObject(obj).ToString(Formatting.None) + "\r\n";

                geoJsonFile.Write(json);

                if (_first)
                    _first = false;
            }

            geoJsonFile.Close();
        }

        private JObject CreateFeatureJsonObject(GeoJsonObject feature)
        {
            dynamic jsonFeature = new JObject();
            jsonFeature.type = "Feature";
            jsonFeature.id = feature.Id;
            jsonFeature.geometry = JObject.Parse(feature.GeoJson);
            
            if (feature.Properties.Count > 0)
            {
                jsonFeature.properties = CreatePropertiesJsonObject(feature);
            }

            dynamic tippecanoe = new JObject();

            tippecanoe.minzoom = feature.MinZoom;
            tippecanoe.maxzoom = feature.MaxZoom;

            jsonFeature.tippecanoe = tippecanoe;

            return jsonFeature;
        }

        private JObject CreatePropertiesJsonObject(GeoJsonObject feature)
        {
            JObject jsonProperties = new();

            jsonProperties.Add(new JProperty("objecttype", feature.ObjectType));

            foreach (var attr in feature.Properties)
            {
                jsonProperties.Add(new JProperty(attr.Name, attr.Value));
            }

            return jsonProperties;
        }

        public void End()
        {
            using StreamWriter geoJsonFile = new StreamWriter(_fileName, true, Encoding.UTF8);

            // End geojson feature collection
            geoJsonFile.WriteLine(" ] }");

            geoJsonFile.Close();
        }
    }
}
