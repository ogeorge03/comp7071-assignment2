using System;
using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models
{
    public class ServiceInvoice
    {
        public int Id { get; set; }

        [Required]
        public int ServiceAppointmentId { get; set; }
        public ServiceAppointment ServiceAppointment { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        public DateTime? PaymentDueDate { get; set; }

        public bool IsPaid { get; set; }
    }
}
