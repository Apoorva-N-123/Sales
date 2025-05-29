using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(255)]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        public string AddressLine2 { get; set; }

        [StringLength(255)]
        public string AddressLine3 { get; set; }

        [Required]
        [StringLength(255)]
        public string City { get; set; }

        [Required]
        [StringLength(255)]
        public string State { get; set; }

        [Required]
        [StringLength(255)]
        public string Country { get; set; }

        [StringLength(10)]
        public string Pincode { get; set; }

        [StringLength(10)]
        public string GSTStateCode { get; set; }

        [StringLength(15)]
        [RegularExpression(@"^\d{15}$", ErrorMessage = "Invalid GSTIN")]
        public string GSTIN { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string WebsiteLink { get; set; }

        [StringLength(255)]
        public string Remarks { get; set; }
    }
}
