using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JTrip.API.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ICollection<LineItemDto> OrderItems { get; set; }
        public string State { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string TransactionMetadata { get; set; }
    }
}
