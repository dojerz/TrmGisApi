using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using ExtensionMethods;

namespace TrmGisApi.DataBase
{
    public class DBQuery
    {
        private Data.AppConfig _AppConfiguration;

        public DBQuery(Data.AppConfig AppConfiguration)
        {
            _AppConfiguration = AppConfiguration;
        }
        /// <summary>
        /// Get base layer properties by GuID -- MAP_LAYERS
        /// </summary>
        /// <param name="LayerGuIds"></param>
        /// <returns></returns>
        public List<Data.Layer> GetLayers(List<string> LayerGuIds)
        {
            string connectionString = _AppConfiguration.GISDataConnectionString;
            List<Data.Layer> layers = new List<Data.Layer>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand query = connection.CreateCommand();
                connection.Open();
                foreach (var layerGuId in LayerGuIds)
                {
                    query.CommandText = "SELECT * FROM MAP_LAYERS WHERE LAYERGUID = '" + layerGuId + "'";
                    using (OracleDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            layers.Add(new Data.Layer
                            {
                                LayerGuId = reader.GetValue(1).DbObjectToString(),
                                LayerName = reader.GetValue(2).DbObjectToString(),
                                LayerType = reader.GetValue(3).DbObjectToString(),
                                TableName = reader.GetValue(4).DbObjectToString(),
                                Description = reader.GetValue(5).DbObjectToString(),
                                Icon = reader.GetValue(6).DbObjectToString(),
                                Radius = reader.GetValue(7).DbObjectToInteger(),
                                Color = reader.GetValue(8).DbObjectToString(),
                                Weight = reader.GetValue(9).DbObjectToString(),
                                Opacity = reader.GetValue(10).DbObjectToDecimal(),
                                Fill = reader.GetValue(11).DbObjectToBoolean(),
                                FillColor = reader.GetValue(12).DbObjectToString(),
                                FillOpacity = reader.GetValue(13).DbObjectToDecimal(),
                                GUIProperties = reader.GetValue(14).DbObjectToString(),
                                ZoomLimit = reader.GetValue(15).DbObjectToInteger()
                            });
                        }
                    }
                }
            }
            return layers;
        }

        public int GetLayerType(string Guid)
        {
            string connectionString = _AppConfiguration.GISDataConnectionString;
            int layerType = 0;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand query = connection.CreateCommand();
                query.CommandText = "SELECT * FROM T_MAP_GUIDINTYPE WHERE LAYERGUID = '" + Guid + "'";
                connection.Open();

                using (OracleDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        layerType = reader.GetValue(1).DbObjectToInteger();
                    }
                }
            }

            return layerType;
        }

        /// <summary>
        /// Affected layerGuIds by collectionGuId
        /// </summary>
        /// <param name="LayerCollectionGuid"></param>
        /// <returns>layerGuIds</returns>
        public List<string> GetLayersInGroup(string LayerCollectionGuid)
        {
            string connectionString = _AppConfiguration.GISDataConnectionString;
            List<string> layerGuIds = new List<string>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand query = connection.CreateCommand();
                query.CommandText = "SELECT * FROM MAP_LAYERCOLLECTION WHERE LAYERCOLLECTIONGUID = '" + LayerCollectionGuid + "'";
                connection.Open();

                using (OracleDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string val = reader.GetValue(2) as string;
                        layerGuIds.Add(val);
                    }
                }
            }

            return layerGuIds;
        }

        /// <summary>
        /// Plain geojson query
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="LayerGuId"></param>
        /// <returns>Geojson string</returns>
        public Data.DBGeojsonObject GetPlainGeojson(string TableName, string LayerGuId)
        {
            string connectionString = _AppConfiguration.GISDataConnectionString;
            string tmpValue = string.Empty;
            string _oID = string.Empty;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand query = connection.CreateCommand();
                query.CommandText = "SELECT * FROM " + TableName + " WHERE LAYERGUID = '" + LayerGuId + "'";
                connection.Open();
                query.InitialLOBFetchSize = _AppConfiguration.GeoJsonLobFetchSize;

                using (OracleDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tmpValue = reader.GetValue(5) as string;
                        _oID = reader.GetValue(1) as string;
                    }
                }
            }
            return new Data.DBGeojsonObject()
            {
                oID = _oID,
                RawGeoJson = tmpValue.ToString()
            };
        }

        /// <summary>
        /// Point query
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="LayerGuId"></param>
        /// <returns>List of points</returns>
        public List<Data.Point> GetSinglePointsSIMPLIFIED(string TableName, string LayerGuId)
        {
            string connectionString = _AppConfiguration.GISDataConnectionString;
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand query = connection.CreateCommand();
            query.CommandText = "SELECT * FROM " + TableName + " WHERE LAYERGUID = '" + LayerGuId + "'";
            connection.Open();

            List<Data.Point> points = new List<Data.Point>();

            try
            {
                using (OracleDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        points.Add(new Data.Point()
                        {
                            PointID = reader.GetValue(1).DbObjectToString(),
                            Latitude = reader.GetValue(10).DbObjectToDecimal(),
                            Longitude = reader.GetValue(9).DbObjectToDecimal(),
                            Descr1 = reader.GetValue(13).DbObjectToString(),
                            Descr2 = reader.GetValue(14).DbObjectToString(),
                            Descr3 = reader.GetValue(15).DbObjectToString()
                        });

                    }
                }
            }
            catch (Exception Ex)
            {
                // throw; ????
            }
            finally { connection.Close(); }

            return points;
        }
        public List<Data.Point> GetSinglePointsDETAILED(string TableName, string LayerGuId)
        {
            string connectionString = _AppConfiguration.GISDataConnectionString;
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand query = connection.CreateCommand();
            query.CommandText = "SELECT * FROM " + TableName + " WHERE LAYERGUID = '" + LayerGuId + "'";
            connection.Open();

            List<Data.Point> points = new List<Data.Point>();

            try
            {
                using (OracleDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        points.Add(new Data.Point()
                        {
                            PointID = reader.GetValue(1).DbObjectToString(),
                            Color = reader.GetValue(3).DbObjectToString(),
                            Weight = reader.GetValue(4).DbObjectToString(),
                            Opacity = reader.GetValue(5).DbObjectToDecimal(),
                            Fill = reader.GetValue(6).DbObjectToBoolean(),
                            FillColor = reader.GetValue(7).DbObjectToString(),
                            FillOpacity = reader.GetValue(8).DbObjectToDecimal(),
                            Latitude = reader.GetValue(10).DbObjectToDecimal(),
                            Longitude = reader.GetValue(9).DbObjectToDecimal(),
                            Icon = reader.GetValue(11).DbObjectToString(),
                            Radius = reader.GetValue(12).DbObjectToInteger(),
                            Descr1 = reader.GetValue(13).DbObjectToString(),
                            Descr2 = reader.GetValue(14).DbObjectToString(),
                            Descr3 = reader.GetValue(15).DbObjectToString()
                        });

                    }
                }
            }
            catch (Exception Ex)
            {
                // throw; ????
            }
            finally { connection.Close(); }

            return points;
        }
        public List<Data.Line> GetSingleLinesDETAILED(string TableName, string LayerGuId)
        {
            string connectionString = _AppConfiguration.GISDataConnectionString;
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand query = connection.CreateCommand();
            query.CommandText = "SELECT * FROM " + TableName + " WHERE LAYERGUID = '" + LayerGuId + "'";
            connection.Open();

            List<Data.Line> lines = new List<Data.Line>();

            try
            {
                using (OracleDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lines.Add(new Data.Line()
                        {
                            LineId = reader.GetValue(1).DbObjectToString(),
                            LayerGuId = reader.GetValue(2).DbObjectToString(),
                            Color = reader.GetValue(3).DbObjectToString(),
                            Weight = reader.GetValue(4).DbObjectToString(),
                            Opacity = reader.GetValue(5).DbObjectToDecimal(),
                            Fill = reader.GetValue(6).DbObjectToBoolean(),
                            FillColor = reader.GetValue(7).DbObjectToString(),
                            FillOpacity = reader.GetValue(8).DbObjectToDecimal(),
                            ALongitude = reader.GetValue(9).DbObjectToDecimal(),
                            ALatitude = reader.GetValue(10).DbObjectToDecimal(),
                            BLongitude = reader.GetValue(11).DbObjectToDecimal(),
                            BLatitude = reader.GetValue(12).DbObjectToDecimal(),
                            Description1 = reader.GetValue(13).DbObjectToString(),
                            Description2 = reader.GetValue(14).DbObjectToString(),
                            Description3 = reader.GetValue(15).DbObjectToString(),
                        });
                    }
                }
            }
            catch (Exception Ex)
            {
                throw;
            }

            return lines;
        }
        public List<Data.Line> GetSingleLinesSIMPLIFIED(string TableName, string LayerGuId)
        {
            string connectionString = _AppConfiguration.GISDataConnectionString;
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand query = connection.CreateCommand();
            query.CommandText = "SELECT * FROM " + TableName + " WHERE LAYERGUID = '" + LayerGuId + "'";
            connection.Open();

            List<Data.Line> lines = new List<Data.Line>();
            try
            {
                using (OracleDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lines.Add(new Data.Line
                        {
                            LineId = reader.GetValue(1).DbObjectToString(),
                            ALongitude = reader.GetValue(9).DbObjectToDecimal(),
                            ALatitude = reader.GetValue(10).DbObjectToDecimal(),
                            BLongitude = reader.GetValue(11).DbObjectToDecimal(),
                            BLatitude = reader.GetValue(12).DbObjectToDecimal(),
                            Description1 = reader.GetValue(13).DbObjectToString(),
                            Description2 = reader.GetValue(14).DbObjectToString(),
                            Description3 = reader.GetValue(15).DbObjectToString(),
                        });
                    }
                }
            }
            catch (Exception Ex)
            {
                throw;
            }
            return lines;
        }


    }
}
