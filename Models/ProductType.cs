using System;
using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class ProductType
    {
        public int ProductTypeId { get; set; }

        [Required(ErrorMessage = "Product Type Code is required.")]
        public string ProductTypeCode { get; set; }

        [Required(ErrorMessage = "Product Type Description is required.")]
        public string ProductTypeDescription { get; set; }
    }
}
