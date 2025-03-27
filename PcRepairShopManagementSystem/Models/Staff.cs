using Microsoft.AspNetCore.Identity;
using System;



namespace PcRepairShopManagementSystem.Models
{
    public class Staff
    {
        public int Id { get; set; }

        // Foreign key to the AspNetUsers table
        public string ApplicationUserId { get; set; }
        public IdentityUser ApplicationUser { get; set; }

        // Staff-specific Details
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public DateTime HireDate { get; set; }
        public string OfficePhone { get; set; }
    }
}
