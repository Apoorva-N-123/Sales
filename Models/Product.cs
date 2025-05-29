using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class Product
    {
        public int ProductId { get; set; } // Auto-incremented ProductId

        [Required]
        public string ProductCode { get; set; }

        [Required]
        public string ProductDescription { get; set; }

        public string CountCode { get; set; }
        public string CountDescription { get; set; }

        public string MixCount { get; set; }

        public string HSNCode { get; set; }
        public string HSNDescription { get; set; }

        public string ProductTypeCode { get; set; }
        public string ProductTypeDescription { get; set; }

        public string VarietyCode { get; set; }
        public string VarietyDescription { get; set; }

        public string KCSCode { get; set; }
        public string KCSDescription { get; set; }

        public string WHCode { get; set; }
        public string WHDescription { get; set; }

        public decimal Blend { get; set; }
        public decimal ConeWeight { get; set; }
        public decimal BagActualWeight { get; set; }
        public decimal BundleWeight { get; set; }

    }
}
