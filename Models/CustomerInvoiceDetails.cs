namespace Assignment2.Models
{
    public class CustomerInvoiceDetails
    {
        public int Id { get; set; }

        public string? Description { get; set; }
        public decimal? ServiceFee { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public DateTime InvoiceDate { get; set; }

    }
}
