﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JTrip.API.ValidationAttributes;

namespace JTrip.API.Dtos
{
    [TouristRouteTitleMustBeDifferentFromDescription]
    public abstract class TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "Title must not be null")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description must not be null")]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
        // Price is calculated as OriginalPrice * DiscountPercent
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Notes { get; set; }
        public double? Rating { get; set; }
        public string TravelDays { get; set; }
        public string TripType { get; set; }
        public string DepartureCity { get; set; }
        public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; } =
            new List<TouristRoutePictureForCreationDto>();
    }
}
