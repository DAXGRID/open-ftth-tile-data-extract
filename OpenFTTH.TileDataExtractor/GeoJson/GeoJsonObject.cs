using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace OpenFTTH.TileDataExtractor.GeoJson
{
    public record GeoJsonObject
    {
        public long Id { get; }
        public string ObjectType { get; }
        public string GeoJson { get; }
        public int MinZoom { get; }
        public int MaxZoom { get; }

        private readonly List<GeoJsonProperty> _properties = new();

        public IReadOnlyList<GeoJsonProperty> Properties => _properties;

        public GeoJsonObject(long id, string objectType, string geoJson, int minZoom, int maxZoom)
        {
            Id = id;
            ObjectType = objectType;
            GeoJson = geoJson;
            MinZoom = minZoom;
            MaxZoom = maxZoom;
        }

        public void AddProperty(string name, string value)
        {
            if (value != null)
                _properties.Add(new GeoJsonProperty(name, value));
        }
    }
}
