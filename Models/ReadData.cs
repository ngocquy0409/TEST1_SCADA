using System.Runtime.InteropServices;

namespace TEST1_SCADA.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ReadData
    {
        public short A1;   // DBW0
        public short A2;   // DBW2
        public short A3;   // DBW4
    }
}
