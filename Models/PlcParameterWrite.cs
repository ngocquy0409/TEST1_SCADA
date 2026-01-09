using S7.Net;
namespace TEST1_SCADA.Models
{
    public class PlcParameterWrite
    {
        // DB2.DBW0
        public short TocDoChuan { get; set; }
        // DB2.DBW2
        public short ThoiGianDungMay { get; set; }
        //DB2.DBW4
        public short ChapNhanGoi_x01s { get; set; }
        // DB2.DBW6
        public short GoiCan_x01s { get; set; }

        // DB2.DBW8
        public short DayGoiCan_x01s { get; set; }

        // DB2.DBW10
        public short CapNhatTuPLC_s { get; set; }

        // DB2.DBW12
        public short CaSo { get; set; }

        // DB2.DBW14 (1..4)
        public short DayChuyen { get; set; }

        // DB2.DBW16 (1..4)
        public short May { get; set; }

        // DB2.DBW18
        public short SanPhamId { get; set; }
    }
}
