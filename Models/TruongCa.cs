using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TEST1_SCADA.Models
{
    public class TruongCa
    {
        [Key]
        public int Id { get; set; }

        public string MaTruongCa { get; set; }

        public string HovaTen { get; set; }
    }
}
