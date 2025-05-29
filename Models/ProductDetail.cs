using System;
using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class ProductDetails
    {
        [Key]
        public int ProductDetailId { get; set; }

        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Product Code is required.")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Product Description is required.")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Order Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Order Quantity must be greater than zero.")]
        public int OrderQuantity { get; set; }

        [Required(ErrorMessage = "Pack Type is required.")]
        public string PackType { get; set; }

        [Required(ErrorMessage = "Rate is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than zero.")]
        public decimal Rate { get; set; }

        public Orders Order { get; set; }
    }
}
