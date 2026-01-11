namespace TEST1_SCADA.Models
{
    public class MonitorContextRequest
    {
        // Model để nhận dữ liệu từ yêu cầu giám sát
        public int Line { get; set; }
        public int Machine { get; set; }
        public int CaSo { get; set; }
        public int PollMs { get; set; }
    }
}
