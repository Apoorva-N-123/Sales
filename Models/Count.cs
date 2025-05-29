using System;
using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class Count
    {
        public int CountId { get; set; }

        [Required(ErrorMessage = "Count Code is required.")]
        public string CountCode { get; set; }

        [Required(ErrorMessage = "Count Description is required.")]
        public string CountDescription { get; set; }
    }
}
