using Stripe;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce529.Models
{
    public enum OrderStatus
    {
        Pending  = 1  , 
        InProgress  , 
        Shipped  , 
        Completed  , 
        Canceled 
    }
    public enum PaymentMethod
    {
        Card = 1  , 
        Cash 
    }
    public enum PaymentStatus
    {
        Pending = 1,
        Completed  , 
        Refuned 
    }
    public class Order
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending; 
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; 
        public decimal TotalPrice { get; set; }

        public string? SessionId { get; set; }
        public string? TransactionId { get; set; }


        public DateTime? ShippedAt { get; set; }
        public string? CarrierName { get; set;  }
        public string? TrackingId { get; set; }

    }
}
