using System;
using System.Collections.Generic;
using System.Linq;
using TrmGisApi.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace TrmGisApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly Data.AppConfig _appConfiguration;
        private readonly GIS.GisServices _gisServices;

        public ValuesController(ILogger<ValuesController> logger, IOptions<Data.AppConfig> appConfiguration, IOptions<Data.MapProperties> mapProperties)
        {
            _logger = logger;
            _gisServices = new GIS.GisServices(mapProperties.Value, appConfiguration.Value);
            _appConfiguration = appConfiguration.Value;
        }

        [GeojsonResponseHeaderFilter]
        [HttpGet("{guid}")]
        public string Get(string guid)
        {
            _logger.LogInformation("-> GeoJson request started: " + guid);
            DataBase.DBQuery dBQuery = new DataBase.DBQuery(_appConfiguration);

            List<Data.Layer> layers = new List<Data.Layer>();


            // Zero if simple layer, otherwise it's a collection
            List<string> lt = dBQuery.GetLayersInGroup(guid);

            if (lt.Count == 0)
            {
                layers = dBQuery.GetLayers(new List<string>() { guid });
            }
            else
            {
                var layersInGroup = dBQuery.GetLayersInGroup(guid);
                layers = dBQuery.GetLayers(layersInGroup);
            }


            GIS.GJson gJson = new GIS.GJson();
            gJson.type = "FeatureCollection";
            gJson.processedlayers = new List<GIS.ProcessedLayer>();
            gJson.guid = guid;

            foreach (var layer in layers)
            {
                // array of names to create filter when create layer tree in GUI
                // 19/03/19 --> guid instead
                //gJson.layernames.Add(layer.LayerName);
                //gJson.layerguids.Add(layer.LayerGuId);



                // originally: with empty TABLENAME in MAP_LAYERS table, default SOURCE TABLE has been set
                // 19/10/07 --> TABLENAME inMAP_LAYERS table is mandatory

                // 19/12/13 friendlyname bekerült, aktív layer listához
                //gJson.layerfriendlynames.Add(layer.LayerName);


                // 20/01/15 --> processedlayer object to encapsulate geojson special properties
                gJson.processedlayers.Add(new GIS.ProcessedLayer()
                {
                    layerguid = layer.LayerGuId,
                    layerfriendlyname = layer.LayerName,
                    zoomlimit = layer.ZoomLimit
                });


                string tableName = _gisServices.SetDataTableName(layer);
                if (layer.LayerType == "point")
                {
                    List<Data.Point> points = new List<Data.Point>();
                    List<GIS.Feature> features = new List<GIS.Feature>();
                    // source table: default name, or defined in map_layers


                    if (string.IsNullOrEmpty(layer.Color))
                    {
                        points = dBQuery.GetSinglePointsDETAILED(tableName, layer.LayerGuId.ToString());
                        features = _gisServices.ToGISGeojsonFeatures(points, layer);
                    }
                    else
                    {
                        points = dBQuery.GetSinglePointsSIMPLIFIED(tableName, layer.LayerGuId.ToString());
                        features = _gisServices.ToGISGeojsonFeaturesSimplePoint(points, layer);
                    }

                    gJson.features.AddRange(features);
                }
                else if (layer.LayerType == "geojson")
                {
                    List<GIS.Feature> features = new List<GIS.Feature>();
                    Data.DBGeojsonObject geojson = dBQuery.GetPlainGeojson("MAP_LAYER_GEOJSONS", layer.LayerGuId.ToString());
                    features = _gisServices.ToGISGeojsonFeatures(geojson, layer);
                    gJson.features.AddRange(features);
                }
                else if (layer.LayerType == "line")
                {
                    List<Data.Line> lines = new List<Data.Line>();
                    List<GIS.Feature> features = new List<GIS.Feature>();

                    if (string.IsNullOrEmpty(layer.Color))
                    {
                        lines = dBQuery.GetSingleLinesDETAILED(tableName, layer.LayerGuId.ToString());
                        features = _gisServices.ToGISGeojsonFeatures(lines, layer);
                    }
                    else
                    {
                        lines = dBQuery.GetSingleLinesSIMPLIFIED(tableName, layer.LayerGuId.ToString());
                        features = _gisServices.ToGISGeojsonFeaturesSimpleLine(lines, layer);
                    }
                    gJson.features.AddRange(features);
                }
            }

            return JsonConvert.SerializeObject(gJson).ToString();// gJson.ToString();
        }
    }
}
