using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sales.Models
{
    [Table("Invoice")]
    public class Invoice
    {
        [Key]
        public int InvoiceNumber { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [StringLength(50)]
        public string DespatchAdviceNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(20)]
        public string GSTIN { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string PayMode { get; set; }

        public string PaymentTerms { get; set; }
        public string BankName { get; set; }

        [Required]
        public decimal TotValue { get; set; }

        [Required]
        public decimal TotGST { get; set; }

        public decimal? FreightAmount { get; set; }
        public decimal? OtherAmount { get; set; }
        public decimal? RoundOfAmount { get; set; }

        [Required]
        public decimal TotInvoiceAmount { get; set; }

        // Navigation Property for Related InvoiceDetails
        public virtual ICollection<InvoiceDetails> InvoiceDetails { get; set; }
    }
}
