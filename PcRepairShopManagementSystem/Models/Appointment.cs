using System;

namespace PcRepairShopManagementSystem.Models
{
    public class Appointment
    {
        // Primary key
        public int Id { get; set; }

        // Foreign key linking to Customer
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        // Foreign key linking to Staff
        public int? StaffId { get; set; }
        public Staff Staff { get; set; }

        // Date and time of the appointment
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }


        // Description of the issue or service requested
        public string IssueTitle { get; set; }

        // Status of the appointment (e.g., Pending, Completed, Canceled)
        public string Status { get; set; }

        public AppointmentDetail AppointmentDetail { get; set; }
    }
}