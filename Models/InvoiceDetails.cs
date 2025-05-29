using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sales.Models
{
    [Table("InvoiceDetails")]
    public class InvoiceDetails
    {
        [Key]
        public int InvoiceDetailsId { get; set; }

        [Required]
        public int InvoiceNumber { get; set; }  // Foreign Key

        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductDescription { get; set; }

        [Required]
        public int OrderQuantity { get; set; }

        [Required]
        [StringLength(50)]
        public string PackType { get; set; }

        [Required]
        public int DespatchQuantity { get; set; }

        [Required]
        public int InvoiceQuantity { get; set; }

        [Required]
        public decimal Rate { get; set; }

        [Required]
        public decimal Val { get; set; }

        [Required]
        public decimal TaxPercentage { get; set; }

        [Required]
        public decimal TaxAmount { get; set; }

        [Required]
        public decimal InvoiceAmount { get; set; }

        // Navigation Property
        [ForeignKey("InvoiceNumber")]
        public virtual Invoice Invoice { get; set; }
    }
}
