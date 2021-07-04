using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JTrip.API.Models;

namespace JTrip.API.Services
{
    public interface ITouristRouteRepository
    {
        Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string keyword, string ratingOperator, int? ratingValue);
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdListAsync(IEnumerable<Guid> ids);
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);
        Task<TouristRoutePicture> GetPictureAsync(int pictureId);
        void AddTouristRoute(TouristRoute touristRoute);
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
        Task<ShoppingCart> GetShoppingCartByUserIdAsync(string userId);
        Task CreateShoppingCartAsync(ShoppingCart shoppingCart);
        Task<bool> SaveAsync();
    }
}
