namespace PcRepairShopManagementSystem.Models
{
    public class Order
    {
        // Primary key
        public int Id { get; set; }

        // Foreign key linking to the Customer who placed the order
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        // Date of the order
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Total amount for the order
        public decimal TotalAmount { get; set; }

        // Shipping address for the order
        public string ShippingAddress { get; set; }

        // Indicates if the order has been shipped
        public bool IsShipped { get; set; }

        // Indicates whether the order is for collection or delivery
        public bool IsForDelivery { get; set; }

        // Navigation property to order details
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
