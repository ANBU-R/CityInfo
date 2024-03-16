using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Model
{
    public class PointOfInterestForCreationDto
    {

        [Required]
        //custom error message with ErrirMessage arqument
        [MaxLength(50, ErrorMessage = "Must be less than 50 char length")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
