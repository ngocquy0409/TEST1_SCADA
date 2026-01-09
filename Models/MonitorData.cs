using System.ComponentModel.DataAnnotations;
// Lưu giữ liệu giám sát từ PLC cho 4 máy trong 1 dây chuyền
namespace TEST1_SCADA.Models
{
    public class MonitorData
    {
        [Key]
        public long Id { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;

        public string DayChuyen { get; set; } = "";
        public string May { get; set; } = "";
        public string Ca { get; set; } = "";
        public string MaTruongCa { get; set; } = "";
        public string MaSanPham { get; set; } = "";

        // Ví dụ các dữ liệu PLC cho 1 máy (sau này bạn mở rộng cho 4 máy)
        public int TocDo { get; set; }
        public float OEE { get; set; }
        public int Stop5s { get; set; }
        public int Stop5m { get; set; }
        public float EmptyPct { get; set; }
        public int Total { get; set; }
        public int Good { get; set; }
        public int Jam { get; set; }
        public int Empty { get; set; }
    }
}
