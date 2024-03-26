using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    /// <summary>
    /// Repository for accessing city and point of interest data.
    /// </summary>
    public class CityInfoRepository : ICityInfoRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CityInfoRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        private readonly CityInfoContext _context;
        public CityInfoRepository(
           CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        /// <inheritdoc/>
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.ToListAsync();
        }
        /// <inheritdoc/>
        /// <summary>
        /// Asynchronously retrieves a collection of cities based on the provided name and search query.
        /// If both <paramref name="name"/> and <paramref name="searchQuery"/> are null or whitespace, 
        /// all cities are returned.
        /// </summary>
        /// <param name="name">Optional parameter representing the name of the city to search for.</param>
        /// <param name="searchQuery">Optional parameter representing additional search criteria for filtering cities.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains an enumerable collection of City objects
        /// that match the specified criteria. If no criteria are provided, all cities may be returned.
        /// </returns>
        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            // Convert DbSet to IQueryable for dynamic querying
            var collection = _context.Cities as IQueryable<City>;

            // If name is provided, filter by exact match
            if (!string.IsNullOrEmpty(name))
            {
                //Trim white spaces
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            // If searchQuery is provided, filter by name or description containing the searchQuery
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(collection => collection.Name.Contains(searchQuery) ||
                (collection.Description != null && collection.Description.Contains
                (searchQuery))); //Description is nullable string
            }

            //Add Pagination
            collection = collection.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Execute query asynchronously and return the result
            return await collection.ToListAsync();

        }
        /// <inheritdoc/>
        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest)
        {
            return
                includePointOfInterest
                ?
                await _context.Cities
                        .Include(c => c.PointsOfInterest)
                        .Where(c => c.Id == cityId)
                        .FirstOrDefaultAsync()
                : await _context.Cities
                        .Where(c => c.Id == cityId)
                        .FirstOrDefaultAsync();
            ;
        }

        /// <inheritdoc/>
        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterests
                 .Where(p => p.Id == pointOfInterestId && p.CityId == cityId)
        .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> CityExistAsync(int cityId)
        {
            return await _context.Cities.Where(c => c.Id == cityId).AnyAsync();
        }

        /// <inheritdoc/>
        public async Task AddPointOfInterestAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                // Adds the point of interest to the city entity.
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SaveChangesAsync()
        {
            // Saves changes asynchronously to the database.
            return await _context.SaveChangesAsync() >= 1;
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }


    }
}
