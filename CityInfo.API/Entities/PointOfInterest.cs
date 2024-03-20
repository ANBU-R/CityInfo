using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [ForeignKey("CityId")]
        // navigation property
        public City? City { get; set; }

        // foreign key

        public int CityId { get; set; }


        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}
