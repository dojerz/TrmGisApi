namespace TrmGisApi.Data
{
    public class Layer : BaseProperties
    {
        public string LayerGuId { get; set; }
        public string LayerName { get; set; }
        public string LayerType { get; set; }
        public string TableName { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Radius { get; set; }
        public string GUIProperties { get; set; }
        /// <summary>
        /// Over Zoomlimit value layer will be hidden, default: 8
        /// </summary>
        public int ZoomLimit { get; set; } = 8;
    }
}
