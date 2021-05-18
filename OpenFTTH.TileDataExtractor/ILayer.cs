using OpenFTTH.TileDataExtractor.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFTTH.TileDataExtractor
{
    public interface ILayer
    {
        string Name { get; }
        List<GeoJsonObject> GetObjects();
    }
}
