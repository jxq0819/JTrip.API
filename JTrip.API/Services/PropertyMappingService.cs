using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper.Configuration.Internal;
using JTrip.API.Dtos;
using JTrip.API.Models;

namespace JTrip.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _touristRoutePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string> {"Id"})},
                {"Title", new PropertyMappingValue(new List<string> {"Title"})},
                {"Rating", new PropertyMappingValue(new List<string> {"Rating"})},
                {"OriginalPrice", new PropertyMappingValue(new List<string> {"OriginalPrice"})}
            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<TouristRouteDto, TouristRoute>(_touristRoutePropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception(
                $"Cannot find exact property mapping instance for <{typeof(TSource)}, {typeof(TDestination)}>");
        }
    }
}
