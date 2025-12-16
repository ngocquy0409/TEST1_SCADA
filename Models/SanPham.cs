using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TEST1_SCADA.Models
{
    public class SanPham
    {
        [Key]
        public int Id { get; set; }

        public string MaSanPham { get; set; }

        public string TenSanPham { get; set; }

    }
}
