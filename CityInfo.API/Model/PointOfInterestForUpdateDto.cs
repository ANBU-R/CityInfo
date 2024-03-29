using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Model
{
    /// <summary>
    /// Data transfer object for updating a point of interest.
    /// </summary>
    public class PointOfInterestForUpdateDto
    {

        /// <summary>
        /// The name of the point of interest.
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name must be less than 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The description of the point of interest (optional).
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

    }
}
