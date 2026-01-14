public class MachineStatusRecord
{
    public int Id { get; set; }

    public int Line { get; set; }           // 1..4
    public DateTime CreatedAt { get; set; } // thời điểm lưu

    // Giá trị decode để bạn query dễ
    public string M1 { get; set; } = "UNKNOWN";
    public string M2 { get; set; } = "UNKNOWN";
    public string M3 { get; set; } = "UNKNOWN";
    public string M4 { get; set; } = "UNKNOWN";

    // Raw byte để debug / phân tích sau này
    public byte RawM1 { get; set; }
    public byte RawM2 { get; set; }
    public byte RawM3 { get; set; }
    public byte RawM4 { get; set; }
}
