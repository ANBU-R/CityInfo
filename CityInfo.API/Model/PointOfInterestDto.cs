namespace CityInfo.API.Model
{

    /// <summary>
    /// Represent Dto of a Point Of Interest.
    /// </summary>
    public class PointOfInterestDto
    {

        /// <summary>
        /// Id of the Point Of Interest.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the Point Of Interest.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Point Of Interest (optional parameter).
        /// </summary>
        public string? Description { get; set; }

    }
}
