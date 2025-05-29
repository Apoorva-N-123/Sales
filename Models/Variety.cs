using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class Variety
    {
        public int VarietyId { get; set; }

        [Required(ErrorMessage = "Variety Code is required.")]
        public string VarietyCode { get; set; }

        [Required(ErrorMessage = "Variety Description is required.")]
        public string VarietyDescription { get; set; }
    }
}
