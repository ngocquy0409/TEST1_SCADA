using System.Text.Json.Serialization;

namespace TEST1_SCADA.Models
{
    public class MonitorLiveDto
    {
        public bool ok { get; set; }
        public string? updatedAt { get; set; }

        public string? maTruongCa { get; set; }
        public string? tenTruongCa { get; set; }
        public string? maSanPham { get; set; }
        public string? tenSanPham { get; set; }

        [JsonPropertyName("M1_Speed")] public int? M1_Speed { get; set; }
        [JsonPropertyName("M1_Oee")] public int? M1_Oee { get; set; }
        [JsonPropertyName("M1_Stop5s")] public int? M1_Stop5s { get; set; }
        [JsonPropertyName("M1_Stop5m")] public int? M1_Stop5m { get; set; }
        [JsonPropertyName("M1_EmptyPct")] public int? M1_EmptyPct { get; set; }
        [JsonPropertyName("M1_Total")] public int? M1_Total { get; set; }
        [JsonPropertyName("M1_Good")] public int? M1_Good { get; set; }
        [JsonPropertyName("M1_Jam")] public int? M1_Jam { get; set; }
        [JsonPropertyName("M1_Empty")] public int? M1_Empty { get; set; }

        [JsonPropertyName("M2_Speed")] public int? M2_Speed { get; set; }
        [JsonPropertyName("M2_Oee")] public int? M2_Oee { get; set; }
        [JsonPropertyName("M2_Stop5s")] public int? M2_Stop5s { get; set; }
        [JsonPropertyName("M2_Stop5m")] public int? M2_Stop5m { get; set; }
        [JsonPropertyName("M2_EmptyPct")] public int? M2_EmptyPct { get; set; }
        [JsonPropertyName("M2_Total")] public int? M2_Total { get; set; }
        [JsonPropertyName("M2_Good")] public int? M2_Good { get; set; }
        [JsonPropertyName("M2_Jam")] public int? M2_Jam { get; set; }
        [JsonPropertyName("M2_Empty")] public int? M2_Empty { get; set; }

        [JsonPropertyName("M3_Speed")] public int? M3_Speed { get; set; }
        [JsonPropertyName("M3_Oee")] public int? M3_Oee { get; set; }
        [JsonPropertyName("M3_Stop5s")] public int? M3_Stop5s { get; set; }
        [JsonPropertyName("M3_Stop5m")] public int? M3_Stop5m { get; set; }
        [JsonPropertyName("M3_EmptyPct")] public int? M3_EmptyPct { get; set; }
        [JsonPropertyName("M3_Total")] public int? M3_Total { get; set; }
        [JsonPropertyName("M3_Good")] public int? M3_Good { get; set; }
        [JsonPropertyName("M3_Jam")] public int? M3_Jam { get; set; }
        [JsonPropertyName("M3_Empty")] public int? M3_Empty { get; set; }

        [JsonPropertyName("M4_Speed")] public int? M4_Speed { get; set; }
        [JsonPropertyName("M4_Oee")] public int? M4_Oee { get; set; }
        [JsonPropertyName("M4_Stop5s")] public int? M4_Stop5s { get; set; }
        [JsonPropertyName("M4_Stop5m")] public int? M4_Stop5m { get; set; }
        [JsonPropertyName("M4_EmptyPct")] public int? M4_EmptyPct { get; set; }
        [JsonPropertyName("M4_Total")] public int? M4_Total { get; set; }
        [JsonPropertyName("M4_Good")] public int? M4_Good { get; set; }
        [JsonPropertyName("M4_Jam")] public int? M4_Jam { get; set; }
        [JsonPropertyName("M4_Empty")] public int? M4_Empty { get; set; }
    }
}
