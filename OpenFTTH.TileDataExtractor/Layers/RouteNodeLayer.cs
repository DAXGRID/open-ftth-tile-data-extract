using NetTopologySuite.IO;
using Npgsql;
using OpenFTTH.TileDataExtractor.GeoJson;
using System.Collections.Generic;
using System.Data;

namespace OpenFTTH.TileDataExtractor
{
    public class RouteNodeLayer : ILayer
    {
        private readonly string _postgresConnectionString;
        private readonly Counter _counter;

        private readonly HashSet<string> _big = new HashSet<string> 
        { 
            "CentralOfficeBig", "CentralOfficeMedium", "CentralOfficeSmall" 
        };

        private readonly HashSet<string> _medium = new HashSet<string> 
        {
            "CabinetBig"
        };

        public string Name => "route_node";

        public RouteNodeLayer(string postgresConnectionString, Counter counter)
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
                    routenode_kind, 
                    routenode_function, 
                    naming_name, 
                    mapping_method,
                    lifecycle_deployment_state
                  from 
                    route_network.route_node 
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
                var function = dbReader.IsDBNull(3) ? null : dbReader[3].ToString();
                var name = dbReader.IsDBNull(4) ? null : dbReader[4].ToString();
                var method = dbReader.IsDBNull(5) ? null : dbReader[5].ToString();
                var status = dbReader.IsDBNull(6) ? null : dbReader[6].ToString();

                var jsonObj = new GeoJsonObject(_counter.GetNext(), Name, geojson, MinZoomLevel(kind, function), MaxZoomLevel(kind, function));

                jsonObj.AddProperty("mrid", mrid);
                jsonObj.AddProperty("kind", kind);
                jsonObj.AddProperty("function", function);
                jsonObj.AddProperty("name", name);
                jsonObj.AddProperty("method", method);
                jsonObj.AddProperty("status", status);

                result.Add(jsonObj);
            }

            return result;
        }

        private int MinZoomLevel(string kind, string function)
        {
            if (_big.Contains(kind))
                return 5;
            else if (_medium.Contains(kind))
                return 12;
            else if (kind != null)
                return 15;
            else
                return 17;
        }

        private int MaxZoomLevel(string kind, string function)
        {
            return 17;
        }

        private IDbConnection GetConnection()
        {
            return new NpgsqlConnection(_postgresConnectionString);
        }
    }
}
