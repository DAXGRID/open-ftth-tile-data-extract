namespace OpenFTTH.TileDataExtractor.GeoJson
{
    public record GeoJsonProperty
    {
        public string Name { get; }
        public object Value { get; }

        public GeoJsonProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
