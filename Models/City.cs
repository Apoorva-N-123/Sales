using System.ComponentModel.DataAnnotations;

namespace Sales.Models
{
    public class City
    {
        // Auto-incremented ID for the City
        public int CityId { get; set; }

        // The name of the City
        [Required(ErrorMessage = "City Name is required.")]
        public string CityName { get; set; }
    }
}
