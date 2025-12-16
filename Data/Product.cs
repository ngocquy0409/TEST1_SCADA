using System.ComponentModel.DataAnnotations;

namespace TEST1_SCADA.Data
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Tên")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [MaxLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
