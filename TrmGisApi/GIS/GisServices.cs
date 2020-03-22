using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ExtensionMethods;
using System.Dynamic;

namespace TrmGisApi.GIS
{
    public class GisServices
    {
        private Data.MapProperties _MapProperties;
        private Data.AppConfig _AppConfig;
        public GisServices(Data.MapProperties MapProperties, Data.AppConfig appConfiguration)
        {
            _MapProperties = MapProperties;
            _AppConfig = appConfiguration;
        }
        /// <summary>
        /// Create standard geojson feature list from Raw geojson features(not like database structured)
        /// </summary>
        /// <param name="RawGeoJson"></param>
        /// <returns></returns>
        public List<Feature> ToGISGeojsonFeatures(Data.DBGeojsonObject RawGeoJson, Data.Layer Layer)
        {
            List<Feature> features = new List<Feature>();
            string JsonString = GeoJsonObjectToString(RawGeoJson);

            var parsed = Newtonsoft.Json.Linq.JObject.Parse(JsonString);
            JArray Val = (JArray)parsed["features"];

            if (Val != null)
            {
                foreach (JToken jfeature in Val)
                {
                    Feature geoFeature = new Feature();
                    geoFeature.friendlyname = Layer.LayerName;
                    geoFeature.guid = Layer.LayerGuId;
                    geoFeature.type = "Feature";

                    var featureGeometry = jfeature["geometry"];
                    var featureProperties = jfeature["properties"];

                    var geometryType = featureGeometry["type"];
                    var coordinates = featureGeometry["coordinates"];

                    //Properties geojsonProperties = (Properties)GetProperties(featureProperties);
                    geoFeature.properties = GetProperties(featureProperties);
                    geoFeature.properties.oID = RawGeoJson.oID;

                    if ((string)geometryType == "Point")
                    {
                        var lat = (decimal)coordinates.Values().ElementAt(0);
                        var lon = (decimal)coordinates.Values().ElementAt(1);
                        geoFeature.geometry = new PointGeometry("Point") { coordinates = new[] { lat, lon } };

                        features.Add(geoFeature);
                    }
                    else if ((string)geometryType == "LineString")
                    {
                        int coordCount = coordinates.Count();
                        decimal[][] lineStringCoordinates = new decimal[coordCount][];
                        for (int i = 0; i < coordCount; i++)
                        {
                            var lat = (decimal)coordinates[i].Values().ElementAt(0);
                            var lon = (decimal)coordinates[i].Values().ElementAt(1);
                            lineStringCoordinates[i] = new decimal[] { lat, lon };
                        }

                        geoFeature.geometry = new LineStringGeometry("LineString") { coordinates = lineStringCoordinates };

                        features.Add(geoFeature);
                    }
                    else if ((string)geometryType == "Polygon")
                    {
                        var polygonCoordinates = coordinates.First ?? "";
                        int coordCount = polygonCoordinates.Count();

                        decimal[][] polyFeatureCoordinates = new decimal[coordCount][];
                        for (int i = 0; i < coordCount; i++)
                        {
                            var lat = (decimal)polygonCoordinates[i].Values().ElementAt(0);
                            var lon = (decimal)polygonCoordinates[i].Values().ElementAt(1);
                            polyFeatureCoordinates[i] = new decimal[] { lat, lon };
                        }

                        // donut polygon capability is unimplemented
                        geoFeature.geometry = new PolygonGeometry("Polygon") { coordinates = new decimal[][][] { polyFeatureCoordinates } };

                        features.Add(geoFeature);
                    }
                    else if ((string)geometryType == "MultiPolygon")
                    {
                        //var polygonCoordinates = coordinates.First ?? "";
                        //int polygonCount = polygonCoordinates.Count();

                        //for (int i = 0; i < polygonCount; i++)
                        //{
                        //    var smallPolygon = polygonCoordinates[i].Values();
                        //    int smallCount = smallPolygon.Count();
                        //    decimal[][] polyFeatureCoordinates = new decimal[polygonCount][];
                        //    for (int j = 0; j < smallCount; j++)
                        //    {
                        //        decimal lat = 0;
                        //        decimal lon = 0;
                        //        if (j % 2 == 0)
                        //        {
                        //            lat = (decimal)polygonCoordinates[i].Values().ElementAt(j);
                        //        }
                        //        else
                        //        {
                        //            lon = (decimal)polygonCoordinates[i].Values().ElementAt(j);
                        //        }
                        //    }

                        //}
                    }
                    else if ((string)geometryType == "MultiPoint")
                    {

                    }
                    else if ((string)geometryType == "MultiLineString")
                    {

                    }
                }
            }

            return features;
        }

        /// <summary>
        /// Create standard geojson feature list from Point objects and properties
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="Layer">Drawing properties defined in Layer table</param>
        /// <returns></returns>
        public List<Feature> ToGISGeojsonFeaturesSimplePoint(List<Data.Point> Points, Data.Layer Layer)
        {
            List<GIS.Feature> features = new List<GIS.Feature>();

            foreach (var dbPoint in Points)
            {
                features.Add(new GIS.Feature("Feature")
                {
                    guid = Layer.LayerGuId,
                    friendlyname = Layer.LayerName,
                    geometry = new GIS.PointGeometry("Point")
                    {
                        coordinates = new decimal[] { dbPoint.Latitude, dbPoint.Longitude }
                    },
                    properties = new GIS.Properties()
                    {
                        oID = dbPoint.PointID,
                        color = Layer.Color,
                        opacity = Layer.Opacity,
                        weight = Layer.Weight,
                        fill = Layer.Fill,
                        fillColor = Layer.FillColor,
                        fillOpacity = Layer.FillOpacity,
                        descr1 = dbPoint.Descr1,
                        descr2 = dbPoint.Descr2,
                        descr3 = dbPoint.Descr3,
                        radius = Layer.Radius,
                        icon = Layer.Icon
                    }
                });
            }
            return features;
        }
        /// <summary>
        /// Create standard geojson feature list from Point objects
        /// </summary>
        /// <param name="Points"></param>
        /// <returns></returns>
        public List<Feature> ToGISGeojsonFeatures(List<Data.Point> Points, Data.Layer Layer)
        {
            List<GIS.Feature> features = new List<GIS.Feature>();

            foreach (var dbPoint in Points)
            {
                features.Add(new GIS.Feature("Feature")
                {
                    friendlyname = Layer.LayerName,
                    guid = Layer.LayerGuId,
                    geometry = new GIS.PointGeometry("Point")
                    {
                        coordinates = new decimal[] { dbPoint.Latitude, dbPoint.Longitude }
                    },
                    properties = new GIS.Properties()
                    {
                        oID = dbPoint.PointID,
                        color = dbPoint.Color,
                        opacity = dbPoint.Opacity,
                        weight = dbPoint.Weight,
                        fill = dbPoint.Fill,
                        fillColor = dbPoint.FillColor,
                        fillOpacity = dbPoint.FillOpacity,
                        descr1 = dbPoint.Descr1,
                        descr2 = dbPoint.Descr2,
                        descr3 = dbPoint.Descr3,
                        radius = dbPoint.Radius,
                        icon = dbPoint.Icon
                    }
                });
            }
            return features;
        }

        public List<Feature> ToGISGeojsonFeaturesSimpleLine(List<Data.Line> Lines, Data.Layer Layer)
        {
            List<GIS.Feature> features = new List<GIS.Feature>();

            foreach (var dbLine in Lines)
            {
                features.Add(new GIS.Feature("Feature")
                {
                    friendlyname = Layer.LayerName,
                    guid = Layer.LayerGuId,
                    geometry = new GIS.LineStringGeometry("LineString")
                    {
                        coordinates = new decimal[][] { new decimal[] { dbLine.ALatitude, dbLine.ALongitude }, new decimal[] { dbLine.BLatitude, dbLine.BLongitude } }
                    },
                    properties = new Properties()
                    {
                        oID = dbLine.LineId,
                        color = Layer.Color,
                        opacity = Layer.Opacity,
                        weight = Layer.Weight,
                        fill = Layer.Fill,
                        fillColor = Layer.FillColor,
                        fillOpacity = Layer.FillOpacity,
                        descr1 = dbLine.Description1,
                        descr2 = dbLine.Description2,
                        descr3 = dbLine.Description3,
                    }
                });
            }

            return features;
        }
        /// <summary>
        /// Create standard geojson feature list from Line objects
        /// </summary>
        /// <param name="Lines"></param>
        /// <returns></returns>
        public List<Feature> ToGISGeojsonFeatures(List<Data.Line> Lines, Data.Layer Layer)
        {
            List<GIS.Feature> features = new List<GIS.Feature>();

            foreach (var dbLine in Lines)
            {
                features.Add(new GIS.Feature("Feature")
                {
                    friendlyname = Layer.LayerName,
                    guid = Layer.LayerGuId,
                    geometry = new GIS.LineStringGeometry("LineString")
                    {
                        coordinates = new decimal[][] { new decimal[] { dbLine.ALatitude, dbLine.ALongitude }, new decimal[] { dbLine.BLatitude, dbLine.BLongitude } }
                    },
                    properties = new Properties()
                    {
                        oID = dbLine.LineId,
                        color = dbLine.Color,
                        opacity = dbLine.Opacity,
                        weight = dbLine.Weight,
                        fill = dbLine.Fill,
                        fillColor = dbLine.FillColor,
                        fillOpacity = dbLine.FillOpacity,
                        descr1 = dbLine.Description1,
                        descr2 = dbLine.Description2,
                        descr3 = dbLine.Description3,
                    }
                });
            }

            return features;
        }

        /// <summary>
        /// Converts raw geojson properties - adds defaults if stroke undefined
        /// </summary>
        /// <param name="RawProperties"></param>
        /// <returns></returns>
        private dynamic GetProperties(JToken RawProperties)
        {
            Random randomIndex = new Random();
            //Properties geoJsonProperties = new Properties();
            dynamic geoJsonProperties = new ExpandoObject();

            int rand = randomIndex.Next(0, _MapProperties.MapColors.Count());
            geoJsonProperties.weight = _MapProperties.DefaultWeight;
            geoJsonProperties.opacity = _MapProperties.DefaultOpacity;
            geoJsonProperties.color = _MapProperties.MapColors[rand];

            if (RawProperties.HasValues)
            {
                foreach (JProperty property in RawProperties)
                {
                    var propName = property.Name.ToUpper();
                    var propValue = property.Value;

                    switch (propName)
                    {
                        case "COLOR":
                            geoJsonProperties.color = (string)propValue;
                            break;
                        case "WEIGHT":
                            geoJsonProperties.weight = (string)propValue;
                            break;
                        case "OPACITY":
                            geoJsonProperties.opacity = ((string)propValue).Replace('.', ',').DbObjectToDecimal();
                            break;
                        case "FILL":
                            geoJsonProperties.fill = Convert.ToBoolean(propValue);
                            break;
                        case "FILLCOLOR":
                            geoJsonProperties.fillColor = (string)propValue;
                            break;
                        case "FILLOPACITY":
                            geoJsonProperties.fillOpacity = ((string)propValue).Replace('.', ',').DbObjectToDecimal();
                            break;
                        case "DESCRIPTION":
                            geoJsonProperties.descr1 = (string)propValue;
                            break;
                        //default:
                        //    geoJsonProperties.UnknowProperty = propValue;
                        //    //geoJsonProperties.descr2 += propValue + ";";
                        //    break;
                        default:
                            //geoJsonProperties.Add(propName, propValue);
                            ((IDictionary<string, object>)geoJsonProperties)[propName] = propValue;
                            //geoJsonProperties[propName] = property.Value;
                            break;

                    }
                }
            }
            // IF BASE PROPERTIES ARE NULL SET THE DEFAULTS
            //if (string.IsNullOrEmpty(geoJsonProperties.color))
            //{
            //    int rand = randomIndex.Next(0, _MapProperties.MapColors.Count());
            //    geoJsonProperties.color = _MapProperties.MapColors[rand];
            //}
            //if (string.IsNullOrEmpty(geoJsonProperties.weight))
            //{
            //    geoJsonProperties.weight = _MapProperties.DefaultWeight;
            //}
            //if (geoJsonProperties.opacity == 0)
            //{
            //    geoJsonProperties.opacity = _MapProperties.DefaultOpacity;
            //}
            return geoJsonProperties;
        }

        /// <summary>
        /// Create valid json string from object
        /// </summary>
        /// <param name="RawGeoJson"></param>
        /// <returns>Valid json string, returns {} if empty</returns>
        private string GeoJsonObjectToString(Data.DBGeojsonObject RawGeoJson)
        {
            if (RawGeoJson.RawGeoJson == null)
            {
                return "{}";
            }
            else
            {
                string JsonString = RawGeoJson.RawGeoJson.ToString();
                if (JsonString == "")
                {
                    return "{}";
                }
                else
                {
                    return JsonString;
                }
            }
        }

        public string SetDataTableName(Data.Layer Layer)
        {
            if (!string.IsNullOrEmpty(Layer.TableName))
            {
                return Layer.TableName;
            }
            else
            {
                if (Layer.LayerType == "point")
                {
                    return _AppConfig.DefaultPointTable;
                }
                else if (Layer.LayerType == "line")
                {
                    return _AppConfig.DefaultLineTable;
                }
                else if (Layer.LayerType == "geojson")
                {
                    return _AppConfig.DefaultGeojsonTable;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
