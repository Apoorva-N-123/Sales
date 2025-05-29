using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sales.Models
{
    public class DespatchDetails
    {
        [Key]
        public int DespatchDetailId { get; set; }

        [Required]
        public int DespatchAdviceNumber { get; set; }

        [ForeignKey("DespatchAdviceNumber")]
        public virtual Despatch Despatch { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderReferenceNumber { get; set; }

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
        public decimal Rate { get; set; }

        [Required]
        public int DespatchQuantity { get; set; }
    }
}