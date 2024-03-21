
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{

    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointOfInterests { get; set; }

        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                 new City("New York")
                 {
                     Id = 1,
                     Description = "The one with big bark"

                 },
                 new City("Cennai")
                 {
                     Id = 2,
                     Description = "The one with  beach"
                 },
                 new City("Paris")
                 {
                     Id = 3,
                     Description = "The one with pointy tower"
                 }

                 );
            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The wild park in the middle of the city"
                },
                new PointOfInterest("Empire State Building")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "A skyscraper in manhatten"
                },
                new PointOfInterest("Marina beach")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "longest beach in india"
                },
                new PointOfInterest("Egmore Museum")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "Has lots of historical items"

                },
                new PointOfInterest("Eiffel Tower")
                {
                    Id = 5,
                    CityId = 3,
                }

                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
