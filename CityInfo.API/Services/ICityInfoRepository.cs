using CityInfo.API.Entities; // Importing the namespace for City and PointOfInterest entities.

namespace CityInfo.API.Services
{
    /// <summary>
    /// Interface for accessing city and point of interest data.
    /// </summary>
    public interface ICityInfoRepository
    {
        /// <summary>
        /// Retrieves all cities asynchronously.
        /// </summary>
        Task<IEnumerable<City>> GetCitiesAsync();

        /// <summary>
        /// Retrieves a city asynchronously by its ID.
        /// </summary>
        /// <param name="cityId">The ID of the city to retrieve.</param>
        /// <param name="includePointOfInterest">Flag to include points of interest for the city.</param>
        Task<City?> GetCityAsync(int cityId, bool includePointOfInterest);

        /// <summary>
        /// Retrieves all points of interest for a city asynchronously.
        /// </summary>
        /// <param name="cityId">The ID of the city to retrieve points of interest for.</param>
        Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId);

        /// <summary>
        /// Retrieves a point of interest for a city asynchronously by its ID.
        /// </summary>
        /// <param name="cityId">The ID of the city containing the point of interest.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest to retrieve.</param>
        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);

        /// <summary>
        /// Checks if a city exists asynchronously by its ID.
        /// </summary>
        /// <param name="cityId">The ID of the city to check.</param>
        Task<bool> CityExistAsync(int cityId);

        /// <summary>
        /// Adds a point of interest to a city asynchronously.
        /// </summary>
        /// <param name="cityId">The ID of the city to add the point of interest to.</param>
        /// <param name="pointOfInterest">The point of interest to add.</param>
        Task AddPointOfInterestAsync(int cityId, PointOfInterest pointOfInterest);

        /// <summary>
        /// Saves changes to the data store asynchronously.
        /// </summary>
        Task<bool> SaveChangesAsync();
    }
}
