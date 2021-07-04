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
using Microsoft.AspNetCore.Authorization;

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
    }
}
