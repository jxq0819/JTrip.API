using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using JTrip.API.Dtos;
using JTrip.API.Helper;
using JTrip.API.Models;
using JTrip.API.ResourceParameters;
using JTrip.API.Services;

namespace JTrip.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes([FromQuery] TouristRouteResourceParameters parameters)
        {
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(parameters.Keyword,
                parameters.RatingOperator, parameters.RatingValue);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("No tourist routes");
            }

            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRoutesDto);
        }

        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        [HttpHead("{touristRouteId}")]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"Tourist route {touristRouteId} not found");
            }

            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            return Ok(touristRouteDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute(
            [FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute("GetTouristRouteById", new {touristRouteId = touristRouteToReturn.Id},
                touristRouteToReturn);
        }

        [HttpPut("{touristRouteId}")]
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

        [HttpPatch("{touristRouteId}")]
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
                    ContentTypes = {"application/problem+json"}
                };
            }

            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{touristRouteId}")]
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
