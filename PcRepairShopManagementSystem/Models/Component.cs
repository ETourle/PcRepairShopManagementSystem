namespace PcRepairShopManagementSystem.Models
{
    public class Component
    {
        // Primary key
        public int Id { get; set; }

        // Name of the component (e.g., "16GB DDR4 RAM")
        public string Name { get; set; }

        // Category of the component (e.g., CPU, GPU, RAM)
        public string Category { get; set; }

        // Detailed description of the component
        public string Description { get; set; }

        // Price of the component
        public decimal Price { get; set; }

        // Current stock quantity available
        public int QuantityInStock { get; set; }

        // Manufacturer information
        public string Manufacturer { get; set; }

        public string ImageUrl { get; set; }


    }
}
