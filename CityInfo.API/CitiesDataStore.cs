using CityInfo.API.Model;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        //public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public CitiesDataStore()
        {
            Cities = new List<CityDto>() {
                new CityDto() { Id = 1, Name = "Chennai", Description = "A conservative south indian city",
                    PointsOfInterest=new List<PointOfInterestDto>(){
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name="Beach"
                    },
                    new PointOfInterestDto()
                    {
                        Id=2,
                        Name="Museum"
                    }
                } },
                new CityDto() { Id = 2, Name="NYC", Description="The One with crazy people",
                PointsOfInterest= new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id=3,
                        Name="Central Park"
                    },
                    new PointOfInterestDto()
                    {
                        Id=4,
                        Name="Empire state building"
                    }
                }


                },
                new CityDto() { Id = 3, Name="Paris",
                    Description="The One with big tower",
                    PointsOfInterest= new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto(){
                        Id=5,
                        Name="Eiffel Tower"
                        }
                    }

                }
            };
        }

        public List<CityDto> Cities { get; }
    }
}
