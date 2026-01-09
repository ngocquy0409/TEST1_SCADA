namespace TEST1_SCADA.Models
{
    public class MonitorData2
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        public string TenDayChuyen { get; set; } = "";
        public string TenMay { get; set; } = "";
        public string Ca { get; set; } = "";

        public int? SanPhamId { get; set; }
        public string MaSanPham { get; set; } = "";
        public string TenSanPham { get; set; } = "";

        public string MaTruongCa { get; set; } = "";
        public string TenTruongCa { get; set; } = "";

        public int Speed { get; set; }
        public int Oee { get; set; }
        public int Stop5s { get; set; }
        public int Stop5m { get; set; }
        public int EmptyPct { get; set; }
        public int Total { get; set; }
        public int Good { get; set; }
        public int Jam { get; set; }
        public int Empty { get; set; }
    }
}
