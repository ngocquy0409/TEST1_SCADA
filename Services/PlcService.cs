using S7.Net;

namespace TEST1_SCADA.Services
{
    public class PlcService
    {
        private readonly ILogger<PlcService> _logger;
        private readonly Plc _plc;

        public PlcService(ILogger<PlcService> logger)
        {
            _logger = logger;
            _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 1);
        }

        public void EnsureConnected()
        {
            if (_plc.IsConnected) return;
            _plc.Open();
        }

        // Read WORD -> int (DBW)
        public int ReadInt16(string address)
        {
            // S7.Net trả về object (thường là ushort đối với DBW)
            var raw = _plc.Read(address);
            if (raw is ushort u) return unchecked((short)u); // nếu PLC dùng signed
            if (raw is short s) return s;
            if (raw is int i) return i;
            return Convert.ToInt32(raw);
        }
    }
}
