namespace TrmGisApi.Data
{
    public class Point : BaseProperties
    {
        public string PointID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Descr1 { get; set; }
        public string Descr2 { get; set; }
        public string Descr3 { get; set; }
        public int Radius { get; set; }
        public string Icon { get; set; }
    }
}
