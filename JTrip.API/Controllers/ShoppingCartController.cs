using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using JTrip.API.Dtos;
using JTrip.API.Models;
using JTrip.API.Services;

namespace JTrip.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        public ShoppingCartController(IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetShoppingCart()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserIdAsync(userId);
            var shoppingCartDto = _mapper.Map<ShoppingCartDto>(shoppingCart);
            return Ok(shoppingCartDto);
        }

        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItem([FromBody] AddShoppingCartItemDto addShoppingCartItemDto)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserIdAsync(userId);
            var touristRoute =
                await _touristRouteRepository.GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            if (touristRoute == null)
            {
                return NotFound($"Tourist route {addShoppingCartItemDto.TouristRouteId} not found");
            }

            var lineItem = new LineItem
            {
                TouristRouteId = addShoppingCartItemDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPercent = touristRoute.DiscountPercent
            };

            await _touristRouteRepository.AddShoppingCartItemAsync(lineItem);
            await _touristRouteRepository.SaveAsync();
            var shoppingCartDto = _mapper.Map<ShoppingCartDto>(shoppingCart);
            return Ok(shoppingCartDto);
        }

        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            var lineItem = await _touristRouteRepository.GetShoppingCartItemByItemIdAsync(itemId);
            if (lineItem == null)
            {
                return NotFound($"Shopping cart item {itemId} not found");
            }

            _touristRouteRepository.DeleteShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}
