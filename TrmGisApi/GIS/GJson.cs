using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrmGisApi.GIS
{
    public class GJson
    {
        public string guid { get; set; }
        public string type { get; set; }
        //public List<string> layerguids = new List<string>();

        //public List<string> layerfriendlynames = new List<string>();

        public List<ProcessedLayer> processedlayers { get; set; }
        public List<Feature> features { get; set; }

        public GJson()
        {
            features = new List<Feature>();
        }
        public GJson(string type) : base()
        {
            this.type = type;
        }
    }

    public class ProcessedLayer
    {
        public string layerguid { get; set; }
        public string layerfriendlyname { get; set; }
        public int zoomlimit { get; set; }
    }

    public class Feature
    {
        public Feature()
        {

        }
        public Feature(string type)
        {
            this.type = type;
        }
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public dynamic properties { get; set; }
        public string friendlyname { get; set; }
        public string guid { get; set; }
    }


    public abstract class Geometry
    {
        public string type { get; set; }
    }

    public abstract class GenericGeometry<T> : Geometry
    {
        public T coordinates { get; set; }
    }

    public class PointGeometry : GenericGeometry<decimal[]>
    {
        public PointGeometry(string type)
        {
            this.type = type;
        }
    }

    public class PointPairsGeometry : GenericGeometry<string[][]>
    {
        public PointPairsGeometry(string type)
        {
            this.type = type;
        }
    }
    public class LineStringGeometry : GenericGeometry<decimal[][]>
    {
        public LineStringGeometry(string type)
        {
            this.type = type;
        }
    }
    /// <summary>
    /// Similar to LineString, but the first and last coordinates must be the same 
    /// </summary>
    public class PolygonGeometry : GenericGeometry<decimal[][][]>
    {
        public PolygonGeometry(string type)
        {
            this.type = type;
        }
    }
    public class MultiPolygonGeometry : GenericGeometry<decimal[][][][]>
    {
        public MultiPolygonGeometry(string type)
        {
            this.type = type;
        }
    }
    public class Properties
    {
        public string color { get; set; }
        public string weight { get; set; }
        public decimal opacity { get; set; }

        public bool fill { get; set; }
        public string fillColor { get; set; }
        public decimal fillOpacity { get; set; }

        public string oID { get; set; }

        public string descr1 { get; set; }
        public string descr2 { get; set; }
        public string descr3 { get; set; }

        public int radius { get; set; }
        public string icon { get; set; }

        public object UnknowProperty { get; set; }

        // Added to bind feature to it's parent layer, when it comes to drawing, will be filtered
        //public string layername { get; set; }
    }
}
