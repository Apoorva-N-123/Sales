using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sales.Models
{
    [Table("Orderss")]
    public class Orderss
    {
        [Key]
        public int OrderId { get; set; } // Auto-incremented

        [Required]
        [StringLength(25, ErrorMessage = "Order Reference Number must be exactly 25 characters long.")]
        [Display(Name = "Order Reference Number")]
        public string OrderReferenceNumber { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        [PastOrTodayDate(ErrorMessage = "Order Date cannot be in the future.")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Order Reference Date")]
        [DataType(DataType.Date)]
        [PastOrTodayDate(ErrorMessage = "Order Reference Date cannot be in the future.")]
        public DateTime OrderReferenceDate { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Payment Mode")]
        public string PayMode { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; } // Navigation property for the related OrderDetails
    }

    // Custom Validation Attribute to check for past or today date
    public class PastOrTodayDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date <= DateTime.Today;
            }
            return false;
        }
    }
}
