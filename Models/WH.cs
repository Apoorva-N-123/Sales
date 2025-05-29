using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class WH
    {
        public int WHId { get; set; }

        [Required(ErrorMessage = "Warehouse Code is required.")]
        public string WHCode { get; set; }

        [Required(ErrorMessage = "Warehouse Description is required.")]
        public string WHDescription { get; set; }
    }
}
