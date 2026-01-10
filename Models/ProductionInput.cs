using System.ComponentModel.DataAnnotations;

namespace TEST1_SCADA.Models
{
    public class ProductionInput
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgaySanXuat { get; set; }

        public int SanPhamId { get; set; }     // FK -> SanPham.Id

        [Range(1, 3)]
        public int CaSo { get; set; }          // Ca 1/2/3 (hoặc lấy từ ShiftConfigs)

        [Required]
        public string DayChuyen { get; set; } = "";  // Line 1/2/3...

       // public int MayId { get; set; }        // FK -> May.Id

        [Range(0, 100000000)]
        public int SanLuongThuc { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

