namespace CityInfo.API.Model
{
    /// <summary>
    ///  Represents a city without point of interest information
    /// </summary>
    public class CityWithoutPointOfInterestDto
    {
        /// <summary>
        /// The ID of the city
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the city
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// An optional description of the city
        /// </summary>
        public string? Description { get; set; }
    }
}
