namespace CityInfo.API.Model
{
    /// <summary>
    /// Represents a data transfer object (DTO) for a city.
    /// </summary>
    public class CityDto
    {

        /// <summary>
        /// This is Id of the city
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This is Name of the city.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// This is optional Description of the city.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// This is number of points of interest in the city.
        /// </summary>
        public int NumberOfPointsOfInterest
        {
            get => PointsOfInterest.Count;
        }

        /// <summary>
        ///  It is the collection of points of interest in the city.
        /// </summary>
        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>();
    }

}
