using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JTrip.API.Helper;
using JTrip.API.Models;

namespace JTrip.API.Services
{
    public interface ITouristRouteRepository
    {
        Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(string orderBy, string keyword, string ratingOperator,
            int? ratingValue, int pageSize, int pageNumber);
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdListAsync(IEnumerable<Guid> ids);
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);
        Task<TouristRoutePicture> GetPictureAsync(int pictureId);
        Task AddTouristRouteAsync(TouristRoute touristRoute);
        Task AddTouristRoutePictureAsync(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
        Task<ShoppingCart> GetShoppingCartByUserIdAsync(string userId);
        Task CreateShoppingCartAsync(ShoppingCart shoppingCart);
        Task AddShoppingCartItemAsync(LineItem lineItem);
        Task<LineItem> GetShoppingCartItemByItemIdAsync(int lineItemId);
        void DeleteShoppingCartItem(LineItem lineItem);
        Task<IEnumerable<LineItem>> GetShoppingCartItemsByItemIdListAsync(IEnumerable<int> ids);
        void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems);
        Task AddOrderAsync(Order order);
        Task<PaginationList<Order>> GetOrdersByUserIdAsync(string userId, int pageSize, int pageNumber);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<bool> SaveAsync();
    }
}
