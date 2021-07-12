using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using JTrip.API.Dtos;
using JTrip.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JTrip.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        public OrdersController(IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var orders = await _touristRouteRepository.GetOrdersByUserIdAsync(userId);
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(ordersDto);
        }
    }
}
