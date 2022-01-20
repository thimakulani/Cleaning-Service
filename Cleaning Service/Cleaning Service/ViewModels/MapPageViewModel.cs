using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cleaning_Service.ViewModels
{
    public class MapPageViewModel
    {
        public class DoctorsLocation
        {
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }
        public MapPageViewModel()
        {
             
        }
        internal List<DoctorsLocation> GetLocation()
        {
            List<DoctorsLocation> Items = new List<DoctorsLocation>()
            {
                new DoctorsLocation(){Latitude = -23.891819,Longitude  = 29.728048},
                new DoctorsLocation(){ Latitude= -23.893084403973774, Longitude = 29.724850804582342},
                new DoctorsLocation(){Latitude = -23.890308312514584,Longitude  = 29.73115935995988},
                new DoctorsLocation(){Latitude = -23.889141, Longitude = 29.727844},
            };
            return Items;
        }
    }
}
