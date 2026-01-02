using System.ComponentModel.DataAnnotations;
namespace TEST1_SCADA.Models
{
    public class Setting_shift
    {
        [Key]
        public int time_start { get; set; }
        public string MaTruongCa { get; set; }

        public string TruongCa { get; set; }


    }
}
