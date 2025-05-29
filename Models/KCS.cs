using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class KCS
    {
        public int KCSId { get; set; }

        [Required(ErrorMessage = "KCS Code is required.")]
        public string KCSCode { get; set; }

        [Required(ErrorMessage = "KCS Description is required.")]
        public string KCSDescription { get; set; }
    }
}
