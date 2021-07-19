using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using JTrip.API.Dtos;
using JTrip.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JTrip.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersController(IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
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

        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            var order = await _touristRouteRepository.GetOrderByIdAsync(orderId);
            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }

        [HttpPost("{orderId}/placeOrder")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId)
        {
            var order = await _touristRouteRepository.GetOrderByIdAsync(orderId);
            order.PaymentProcessing();
            await _touristRouteRepository.SaveAsync();
            var httpClient = _httpClientFactory.CreateClient();
            var url = @"http://123.56.149.216/api/FakePaymentProcess?icode={0}&orderNumber={1}&returnFault={2}";
            var response = await httpClient.PostAsync(string.Format(url, "72E32CD895F289DD", order.Id, false), null);
            var isApproved = false;
            var transactionMetadata = "";
            if (response.IsSuccessStatusCode)
            {
                transactionMetadata = await response.Content.ReadAsStringAsync();
                var jsonObject = (JObject) JsonConvert.DeserializeObject(transactionMetadata);
                isApproved = jsonObject["approved"].Value<bool>();
            }

            if (isApproved)
            {
                order.PaymentApprove();
            }
            else
            {
                order.PaymentReject();
            }

            order.TransactionMetadata = transactionMetadata;
            await _touristRouteRepository.SaveAsync();
            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }
    }
}
