using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Foreign Key to Orderss

        [Required]
        [MaxLength(50)]
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }

        [Required]
        [Display(Name = "Order Quantity")]
        public int OrderQuantity { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Pack Type")]
        public string PackType { get; set; }

        [Required]
        [Display(Name = "Rate")]
        public decimal Rate { get; set; }

        public Orderss Order { get; set; } // Navigation property to the Orderss table
    }
}
