using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Pincode { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
