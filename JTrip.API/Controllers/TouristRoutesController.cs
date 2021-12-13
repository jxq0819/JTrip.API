using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JTrip.API.Dtos;
using JTrip.API.Helper;
using JTrip.API.Models;
using JTrip.API.ResourceParameters;
using JTrip.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;

namespace JTrip.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
        }

        private string GenerateTouristRouteResourceUrl(TouristRouteResourceParameters touristRouteResourceParameters,
            PaginationResourceParameters paginationResourceParameters, ResourceUrlType resourceUrlType)
        {
            return resourceUrlType switch
            {
                ResourceUrlType.PreviousPage => _urlHelper.Link("GetTouristRoutes", new
                {
                    orderBy = touristRouteResourceParameters.OrderBy,
                    keyword = touristRouteResourceParameters.Keyword,
                    rating = touristRouteResourceParameters.Rating,
                    fields = touristRouteResourceParameters.Fields,
                    pageNumber = paginationResourceParameters.PageNumber - 1,
                    pageSize = paginationResourceParameters.PageSize
                }),
                ResourceUrlType.NextPage => _urlHelper.Link("GetTouristRoutes", new
                {
                    orderBy = touristRouteResourceParameters.OrderBy,
                    keyword = touristRouteResourceParameters.Keyword,
                    rating = touristRouteResourceParameters.Rating,
                    fields = touristRouteResourceParameters.Fields,
                    pageNumber = paginationResourceParameters.PageNumber + 1,
                    pageSize = paginationResourceParameters.PageSize
                }),
                _ => _urlHelper.Link("GetTouristRoutes", new
                {
                    orderBy = touristRouteResourceParameters.OrderBy,
                    keyword = touristRouteResourceParameters.Keyword,
                    rating = touristRouteResourceParameters.Rating,
                    fields = touristRouteResourceParameters.Fields,
                    pageNumber = paginationResourceParameters.PageNumber,
                    pageSize = paginationResourceParameters.PageSize
                })
            };
        }

        private IEnumerable<LinkDto> CreateLinkForTouristRoute(Guid touristRouteId, string fields)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(Url.Link("GetTouristRouteById", new { touristRouteId, fields }), "self", "GET"),
                new LinkDto(Url.Link("UpdateTouristRoute", new { touristRouteId }), "update", "PUT"),
                new LinkDto(Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }), "partially_update",
                    "PATCH"),
                new LinkDto(Url.Link("DeleteTouristRoute", new { touristRouteId }), "delete", "DELETE"),
                new LinkDto(Url.Link("GetPictureListForTouristRoute", new { touristRouteId }), "get_pictures",
                    "GET"),
                new LinkDto(Url.Link("CreateTouristRoutePicture", new { touristRouteId }), "create_picture", "POST")
            };
            return links;
        }

        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters touristRouteResourceParameters,
            [FromQuery] PaginationResourceParameters paginationResourceParameters)
        {
            if (!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(touristRouteResourceParameters
                .OrderBy))
            {
                return BadRequest("Order by parameters are inappropriate");
            }

            if (!_propertyMappingService.IsPropertyExists<TouristRouteDto>(touristRouteResourceParameters.Fields))
            {
                return BadRequest("Field parameters are inappropriate");
            }

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(
                touristRouteResourceParameters.OrderBy,
                touristRouteResourceParameters.Keyword,
                touristRouteResourceParameters.RatingOperator,
                touristRouteResourceParameters.RatingValue,
                paginationResourceParameters.PageSize,
                paginationResourceParameters.PageNumber);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("No tourist routes");
            }

            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            var previousPageLink = touristRoutesFromRepo.HasPrevious
                ? GenerateTouristRouteResourceUrl(touristRouteResourceParameters, paginationResourceParameters,
                    ResourceUrlType.PreviousPage)
                : null;
            var nextPageLink = touristRoutesFromRepo.HasNext
                ? GenerateTouristRouteResourceUrl(touristRouteResourceParameters, paginationResourceParameters,
                    ResourceUrlType.NextPage)
                : null;

            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPages
            };
            Response.Headers.Add("x-pagination", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(touristRoutesDto.ShapeData(touristRouteResourceParameters.Fields));
        }

        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        [HttpHead("{touristRouteId}")]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId,
            [FromQuery] TouristRouteResourceParameters touristRouteResourceParameters)
        {
            if (!_propertyMappingService.IsPropertyExists<TouristRouteDto>(touristRouteResourceParameters.Fields))
            {
                return BadRequest("Field parameters are inappropriate");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"Tourist route {touristRouteId} not found");
            }

            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            var linksDto = CreateLinkForTouristRoute(touristRouteId, touristRouteResourceParameters.Fields);
            var result =
                touristRouteDto.ShapeData(touristRouteResourceParameters.Fields) as IDictionary<string, object>;
            result.Add("links", linksDto);
            return Ok(result);
        }

        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute(
            [FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            await _touristRouteRepository.AddTouristRouteAsync(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            var linksDto = CreateLinkForTouristRoute(touristRouteModel.Id, null);
            var result = touristRouteToReturn.ShapeData(null) as IDictionary<string, object>;
            result.Add("links", linksDto);
            return CreatedAtRoute("GetTouristRouteById", new { touristRouteId = result["Id"] },
                result);
        }

        [HttpPut("{touristRouteId}", Name = "UpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute([FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"Tourist route {touristRouteId} not found");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute([FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"Tourist route {touristRouteId} not found");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch);
            if (!TryValidateModel(touristRouteToPatch))
            {
                var problemDetail = new ValidationProblemDetails(ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "Validation errors occured",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Detail = "Please read error messages",
                    Instance = HttpContext.Request.Path
                };
                problemDetail.Extensions.Add("traceId", HttpContext.TraceIdentifier);
                return new UnprocessableEntityObjectResult(problemDetail)
                {
                    ContentTypes = { "application/problem+json" }
                };
            }

            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"Tourist route {touristRouteId} not found");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("({touristRouteIds})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoutesByIds(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] [FromRoute]
            IEnumerable<Guid> touristRouteIds)
        {
            if (touristRouteIds == null)
            {
                return BadRequest();
            }

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIdListAsync(touristRouteIds);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}
