using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JTrip.API.Models;

namespace JTrip.API.Services
{
    public class MockTouristRouteRepository : ITouristRouteRepository
    {
        private List<TouristRoute> _routes;

        public MockTouristRouteRepository()
        {
            if (_routes == null)
            {
                InitialiseTouristRoutes();
            }
        }

        private void InitialiseTouristRoutes()
        {
            _routes = new List<TouristRoute>
            {
                new TouristRoute
                {
                    Id = Guid.NewGuid(),
                    Title = "Mountain Huang",
                    Description = "Mountain Huang is fun",
                    OriginalPrice = 1299,
                    Features = "<p>Food, accommodation, transport, shopping and entertainment</p>",
                    Fees = "<p>Transport expenses are not included</p>",
                    Notes = "<p>Stay safe</p>"
                },
                new TouristRoute
                {
                    Id = Guid.NewGuid(),
                    Title = "Mountain Hua",
                    Description = "Mountain Hua is fun",
                    OriginalPrice = 1299,
                    Features = "<p>Food, accommodation, transport, shopping and entertainment</p>",
                    Fees = "<p>Transport expenses are not included</p>",
                    Notes = "<p>Stay safe</p>"
                }
            };
        }

        public IEnumerable<TouristRoute> GetTouristRoutes()
        {
            return _routes;
        }

        public TouristRoute GetTouristRoute(Guid touristRouteId)
        {
            return _routes.FirstOrDefault(n => n.Id == touristRouteId);
        }
    }
}
