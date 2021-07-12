using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Stateless;

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

    public enum OrderStateTriggerEnum
    {
        PlaceOrder,
        Approve,
        Reject,
        Cancel,
        Return
    }

    public class Order
    {
        public Order()
        {
            StateMachineInit();
        }

        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string TransactionMetadata { get; set; }
        private StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine;

        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>(() => State, s => State = s);

            _machine.Configure(OrderStateEnum.Pending)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);

            _machine.Configure(OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);

            _machine.Configure(OrderStateEnum.Declined)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing);

            _machine.Configure(OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refund);
        }

        public void PaymentProcessing()
        {
            _machine.Fire(OrderStateTriggerEnum.PlaceOrder);
        }

        public void PaymentApprove()
        {
            _machine.Fire(OrderStateTriggerEnum.Approve);
        }

        public void PaymentReject()
        {
            _machine.Fire(OrderStateTriggerEnum.Reject);
        }
    }
}
