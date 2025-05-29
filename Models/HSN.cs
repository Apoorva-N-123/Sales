using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class HSN
    {
        [Key]
        public int HSNId { get; set; }

        [Required(ErrorMessage = "HSN Code is required.")]
        [RegularExpression(@"^\d{4}(\d{2}(\d{2})?)?$", ErrorMessage = "HSN Code must be 4, 6, or 8 digits.")]
        public string HSNCode { get; set; }

        [Required(ErrorMessage = "HSN Description is required.")]
        public string HSNDescription { get; set; }
    }
}
