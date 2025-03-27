namespace PcRepairShopManagementSystem.Models
{
    public class OrderDetail
    {
        // Primary key
        public int Id { get; set; }

        // Foreign key linking to the Order
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // Foreign key linking to the Component being purchased
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public string ComponentName { get; set; }

        // Quantity of the component in the order
        public int Quantity { get; set; }

        // Price per unit at the time of the order
        public decimal PricePerUnit { get; set; }
    }
}
