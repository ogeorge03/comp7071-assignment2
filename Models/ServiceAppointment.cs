using System;
using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models
{
    public class ServiceAppointment
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Customer_Service Service { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public string? Notes { get; set; }
    }
}
