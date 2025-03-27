using Microsoft.AspNetCore.Identity;
using System;


namespace PcRepairShopManagementSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

        // Foreign key to the AspNetUsers table
        public string ApplicationUserId { get; set; }
        public IdentityUser ApplicationUser { get; set; }

        // Extended Customer Details
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Address Information
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Additional Information
        public DateTime? DateOfBirth { get; set; }
               
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
