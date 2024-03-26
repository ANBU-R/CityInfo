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
        /// Asynchronously retrieves a collection of cities based on the provided name and search query.
        /// </summary>
        /// <param name="name">Optional parameter representing the name of the city to search for.</param>
        /// <param name="searchQuery">Optional parameter representing additional search criteria for filtering cities.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains an enumerable collection of City objects
        /// that match the specified criteria. If no criteria are provided, all cities may be returned.
        /// </returns>

        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? serachQuery, int pageNumber, int pageSize);

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

        /// <summary>
        /// Deletes a point of interest.
        /// </summary>
        /// <param name="pointOfInterest">The point of interest to delete.</param>

        void DeletePointOfInterest(PointOfInterest pointOfInterest);


    }
}
