using System.ComponentModel.DataAnnotations;

namespace PcRepairShopManagementSystem.Models
{
    public class AppointmentDetail
    {
        // Using the AppointmentId as both the primary key and the foreign key.
        [Key]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        // Extra details about the device for the appointment.
        public string Title { get; set; }        // Title of the device issue or service needed
        public string? Category { get; set; }     // e.g., Laptop, Graphics Card, Printer, SOftware  issue etc.   
        public string? SoftwareTitle { get; set; } // Title of the software (if applicable)
        public string? Brand { get; set; }        // e.g., Dell, HP, Apple
        public string? Model { get; set; }        // Specific model name/number
        public string? SerialNumber { get; set; } // Device serial number (if applicable)
        public string? AdditionalNotes { get; set; }  // Any extra notes regarding the device or issue
    }
}
