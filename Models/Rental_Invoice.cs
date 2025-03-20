namespace Assignment2.Models
{
    public class Rental_Invoice
    {
        public int Id { get; set; }
        public Renter? Renter { get; set; }
        public Asset? Asset { get; set; }
        public Renter_Asset_Agreement? Agreement { get; set; }
        public decimal rental_fee { get; set; }
        public decimal tax {  get; set; }
        public decimal adjustment { get; set; }
        public string? adjustment_reason { get; set; }
        public DateTime Invoice_Date { get; set; }
        public DateTime? Payment_Due_Date { get; set; }

    }
}
