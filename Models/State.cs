using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sales.Models
{
    [Table("State")] // Maps this model to the "State" table
    public class State
    {
        [Key]
        public int StateId { get; set; }

        [Required]
        [MaxLength(100)]
        public string StateName { get; set; }

        [Required]
        [MaxLength(255)]
        public string StateFlag { get; set; }

        [Required]
        public decimal GSTStateCode { get; set; }
    }
}
