namespace TrmGisApi.Data
{
    public class Line : BaseProperties
    {
        public string LineId { get; set; }
        public string LayerGuId { get; set; }
        public decimal ALatitude { get; set; }
        public decimal BLatitude { get; set; }
        public decimal ALongitude { get; set; }
        public decimal BLongitude { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
    }
}
