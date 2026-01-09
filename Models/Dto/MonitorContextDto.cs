namespace TEST1_SCADA.Models.Dto
{
    public class MonitorContextDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        public string TenDayChuyen { get; set; } = "";
        public string TenMay { get; set; } = "";
        public string Ca { get; set; } = "";

        public int? SanPhamId { get; set; }
        public string MaSanPham { get; set; } = "";
        public string TenSanPham { get; set; } = "";

        public string MaTruongCa { get; set; } = "";
        public string TenTruongCa { get; set; } = "";
    }
}
