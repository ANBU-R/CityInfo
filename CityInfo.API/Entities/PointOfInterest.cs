using System.ComponentModel.DataAnnotations; // Importing the namespace for data annotations.
using System.ComponentModel.DataAnnotations.Schema; // Importing the namespace for schema annotations.

namespace CityInfo.API.Entities
{
    /// <summary>
    /// Represents a point of interest entity.
    /// </summary>
    public class PointOfInterest
    {
        /// <summary>
        /// Gets or sets the ID of the point of interest (primary key).
        /// </summary>
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the point of interest.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the point of interest.
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the associated city of the point of interest.
        /// </summary>
        [ForeignKey("CityId")]
        public City? City { get; set; }

        /// <summary>
        /// Gets or sets the foreign key referencing the city to which the point of interest belongs.
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointOfInterest"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the point of interest.</param>
        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}
