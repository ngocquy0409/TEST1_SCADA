namespace TEST1_SCADA.Models
{
    public class MonitorContextRequest
    {
        public int Line { get; set; }
        public int Machine { get; set; }
        public int CaSo { get; set; }
        public int PollMs { get; set; }
    }
}
