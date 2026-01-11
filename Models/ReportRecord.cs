using System;

namespace TEST1_SCADA.Models
{
    public class ReportRecord
    {
        // model chứa dữ liệu báo cáo hàng ngày cho mỗi máy theo 15 chỉ số 
        public long Id { get; set; }

        // Ngữ cảnh báo cáo
        public int Line { get; set; }         // 1..4
        public int Machine { get; set; }      // 1..4
        public int CaSo { get; set; }         // 1..3
        public DateTime NgayBaoCao { get; set; } = DateTime.Today;  // lọc theo ngày

        // Thời điểm lưu (lần đọc)
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //  15 chỉ số (dùng decimal cho có phần thập phân) 
        public decimal MTTR { get; set; }       // MTTR = Mean Time To Repair 
        public decimal MTBF { get; set; }       // MTBF = Mean Time Between Failures
        public decimal StopPct { get; set; }    // Số phần trăm thời gian dừng máy
        public decimal FaultPct { get; set; }   // Số phần trăm thời gian lỗi
        public decimal A { get; set; }          // Availability: A = 1 - StopPct - FaultPct

        public decimal SpeedLossPct { get; set; }   // Tỷ lệ tổn thất tốc độ
        public decimal Vtb { get; set; }            // Vtb = 1 - SpeedLossPct
        public decimal MinorStopPct { get; set; }   // Tỷ lệ dừng máy nhỏ
        public decimal P { get; set; }              // Performance: P = 1 - MinorStopPct: tỉ lệ hiệu suất

        public decimal SpicePct { get; set; }       // Gói cấn gia vị   
        public decimal EmptyPct { get; set; }       // Gói rỗng
        public decimal Q { get; set; }              // Quality: Q = 1 - SpicePct - EmptyPct

        public decimal OEE1 { get; set; }           // OEE1: đánh giá tổng thể 
        public decimal OEE2 { get; set; }           // OEE2: đánh giá hiệu suất
        public decimal OEE3 { get; set; }           // OEE3: đánh giá chất lượng
    }
}
