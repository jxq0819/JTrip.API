using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JTrip.API.Dtos;

namespace JTrip.API.ValidationAttributes
{
    public class TouristRouteTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var touristRouteDto = (TouristRouteForManipulationDto) validationContext.ObjectInstance;
            if (touristRouteDto.Title == touristRouteDto.Description)
            {
                return new ValidationResult("Title and description must be different",
                    new[] {"TouristRouteForManipulationDto"});
            }

            return ValidationResult.Success;
        }
    }
}
