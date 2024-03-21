using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;
        public CityInfoRepository(
           CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.ToListAsync();
        }

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

        public async Task<PointOfInterest?> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterests
                 .Where(p => p.Id == pointOfInterestId && p.CityId == cityId)
        .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }
    }
}
