using Npgsql;
using OpenFTTH.TileDataExtractor.GeoJson;
using System.Collections.Generic;
using System.Data;

namespace OpenFTTH.TileDataExtractor
{
    public class RouteSegmentLayer : ILayer
    {
        private readonly string _postgresConnectionString;
        private readonly Counter _counter;

        public string Name => "route_segment";

        public RouteSegmentLayer(string postgresConnectionString, Counter counter)
        {
            _postgresConnectionString = postgresConnectionString;
            _counter = counter;
        }

        public List<GeoJsonObject> GetObjects()
        {
            List<GeoJsonObject> result = new();

            using var dbConn = GetConnection();
            dbConn.Open();

            using var dbCmd = dbConn.CreateCommand();

            dbCmd.CommandText =
                @"select 
                    mrid, 
                    ST_AsGeoJSON(ST_Transform(coord,4326)) as coord, 
                    routesegment_kind, 
                    mapping_method,
                    lifecycle_deployment_state
                  from 
                    route_network.route_segment 
                  where
                    coord is not null and
                    marked_to_be_deleted = false
                ";

            using var dbReader = dbCmd.ExecuteReader();

            while (dbReader.Read())
            {
                var mrid = dbReader[0].ToString();
                var geojson = dbReader[1].ToString();
                var kind = dbReader.IsDBNull(2) ? null : dbReader[2].ToString();
                var method = dbReader.IsDBNull(3) ? null : dbReader[3].ToString();
                var status = dbReader.IsDBNull(4) ? null : dbReader[4].ToString();

                var jsonObj = new GeoJsonObject(_counter.GetNext(), Name, geojson, 12, 17);

                jsonObj.AddProperty("mrid", mrid);
                jsonObj.AddProperty("kind", kind);
                jsonObj.AddProperty("method", method);
                jsonObj.AddProperty("status", status);

                result.Add(jsonObj);
            }

            return result;
        }

        private IDbConnection GetConnection()
        {
            return new NpgsqlConnection(_postgresConnectionString);
        }
    }
}
