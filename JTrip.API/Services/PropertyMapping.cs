using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JTrip.API.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;
        }
    }
}
