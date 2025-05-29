using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sales.Models
{
    [Table("despatch")]
    public class Despatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DespatchAdviceNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DespatchAdviceDate { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(15)]
        public string GSTIN { get; set; }

        [Required]
        [StringLength(500)]
        public string DeliveryAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        public virtual ICollection<DespatchDetails> DespatchDetails { get; set; } = new List<DespatchDetails>();
    }
}