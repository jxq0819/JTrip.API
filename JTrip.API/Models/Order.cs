using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JTrip.API.Models
{
    public enum OrderStateEnum
    {
        Pending,
        Processing,
        Completed,
        Declined,
        Cancelled,
        Refund
    }

    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string TransactionMetadata { get; set; }
    }
}
