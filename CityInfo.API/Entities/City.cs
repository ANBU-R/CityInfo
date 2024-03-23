using System.ComponentModel.DataAnnotations; // Importing the namespace for data annotations.
using System.ComponentModel.DataAnnotations.Schema; // Importing the namespace for schema annotations.

namespace CityInfo.API.Entities
{
    /// <summary>
    /// Represents a city entity.
    /// </summary>
    public class City
    {
        /// <summary>
        /// Gets or sets the ID of the city (primary key).
        /// </summary>
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the city.
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the collection of points of interest associated with the city.
        /// </summary>
        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
            = new List<PointOfInterest>();

        /// <summary>
        /// Initializes a new instance of the <see cref="City"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the city.</param>
        public City(string name)
        {
            Name = name;
        }
    }
}
