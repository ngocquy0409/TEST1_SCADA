// Lưu dữ liệu của trang cài đặt sản xuất
using System.ComponentModel.DataAnnotations;
namespace TEST1_SCADA.Models
{
    public class ShiftConfig
    {
        public int Id { get; set; }

        [Range(1,3)]
        public int CaSo { get; set; }  // Ca số: 1, 2, hoặc 3
        [Range(0, 23)] // Giờ bắt đầu từ 0 đến 23
        public int GioBatDau { get; set; }  // Giờ bắt đầu ca
        [Range(0, 59)] // Phút bắt đầu từ 0 đến 59
        public int PhutBatDau { get; set; } // Phút bắt đầu ca
        public int TruongCaId { get; set; } // Khóa ngoại đến Trưởng ca

    }
}
