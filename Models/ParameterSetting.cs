using System.ComponentModel.DataAnnotations;

namespace TEST1_SCADA.Models
{
    public class ParameterSetting
    {
        // Database lưu trữ thông tin của thông số người dùng nhập vào
        public int Id { get; set; }

        public string TenDayChuyen { get; set; } = "";

        public string TenMay { get; set; } = "";
        public string Ca { get; set; } = "";
        public int? SanPhamId { get; set; }      // chọn theo mã SP
        public string MaSanPham { get; set; } = "";
        public string TenSanPham { get; set; } = "";

        public bool CaiDatThamSo { get; set; } = true;  // radio: Cài đặt / Không cài đặt
        public int? TocDoChuan { get; set; }            // gói/phút
        public int? ThoiGianDungMay { get; set; }       // s
        public int? ThoiGianChapNhanGoi { get; set; }   // x0.1s
        public int? ThoiGianGoiCan { get; set; }        // x0.1s
        public int? ThoiGianDayGoiCan { get; set; }     // x0.1s
        public int? ThoiGianCapNhatTuPLC { get; set; }  // s

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
