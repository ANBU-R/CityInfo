using CityInfo.API.Model;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current {get;} =  new CitiesDataStore();
        public CitiesDataStore()
        {
            Cities = new List<CityDto>() { 
                new CityDto() { Id = 1, Name = "Chennai", Description = "A conservative south indian city" }, 
                new CityDto() { Id = 2, Name="NYC", Description="The One with crazy people"},
                new CityDto() { Id = 3, Name="Paris", Description="The One with big tower"}
            };
        }

        public List<CityDto> Cities { get; }
    }
}
