namespace TrmGisApi.Data
{
    public class AppConfig
    {
        public string GISDataConnectionString { get; set; }
        public int GeoJsonLobFetchSize { get; set; }
        public string DefaultPointTable { get; set; }
        public string DefaultLineTable { get; set; }
        public string DefaultGeojsonTable { get; set; }
    }
}
