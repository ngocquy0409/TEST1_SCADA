using System.ComponentModel.DataAnnotations.Schema;

namespace TEST1_SCADA.Models
{
    public class MonitorRecord
    {
        public long Id { get; set; }

        // ===== Ngữ cảnh người dùng chọn để giám sát =====
        public int Line { get; set; }        // 1..4
        public int Machine { get; set; }     // 1..4 (máy được chọn để “focus”)
        public int CaSo { get; set; }        // 1..3
        public int PollMs { get; set; }      // chu kỳ đọc ms (lấy từ UI / hoặc từ PLC setting)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== Liên kết “ID” để lấy tên =====
        public int? TruongCaId { get; set; }    // FK -> TruongCa.Id
        public TruongCa? TruongCa { get; set; }  // lấy tên trưởng ca theo TruongCaId   

        public int? SanPhamId { get; set; }     // FK -> SanPham.Id 
        public SanPham? SanPham { get; set; }   // lấy mã/tên sp theo SanPhamId

        // (PLC) dữ liệu máy 1..4 - để trống/nullable, sau xử lý PLC mới fill
        // Máy 1 
        public int? M1_Speed { get; set; }      // gói/phút
        public int? M1_Oee { get; set; }        // %OEE
        public int? M1_Stop5s { get; set; }     // số lần dừng >5s
        public int? M1_Stop5m { get; set; }     // số lần dừng >5m
        public int? M1_EmptyPct { get; set; }   // % thời gian rỗng
        public int? M1_Total { get; set; }      // tổng gói
        public int? M1_Good { get; set; }       // gói đạt
        public int? M1_Jam { get; set; }        // gói lỗi
        public int? M1_Empty { get; set; }      //  gói rỗng
        // Máy 2
        public int? M2_Speed { get; set; }
        public int? M2_Oee { get; set; }
        public int? M2_Stop5s { get; set; }
        public int? M2_Stop5m { get; set; }
        public int? M2_EmptyPct { get; set; }
        public int? M2_Total { get; set; }
        public int? M2_Good { get; set; }
        public int? M2_Jam { get; set; }
        public int? M2_Empty { get; set; }

        // Máy 3
        public int? M3_Speed { get; set; }
        public int? M3_Oee { get; set; }
        public int? M3_Stop5s { get; set; }
        public int? M3_Stop5m { get; set; }
        public int? M3_EmptyPct { get; set; }
        public int? M3_Total { get; set; }
        public int? M3_Good { get; set; }
        public int? M3_Jam { get; set; }
        public int? M3_Empty { get; set; }

        // Máy 4
        public int? M4_Speed { get; set; }
        public int? M4_Oee { get; set; }
        public int? M4_Stop5s { get; set; }
        public int? M4_Stop5m { get; set; }
        public int? M4_EmptyPct { get; set; }
        public int? M4_Total { get; set; }
        public int? M4_Good { get; set; }
        public int? M4_Jam { get; set; }
        public int? M4_Empty { get; set; }
    }
}
