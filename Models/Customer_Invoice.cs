namespace MVCSampleApp.Models
{
    public class Customer_Invoice
    {

        public int Id { get; set; }

        public Customer_Service_Scheduled Service_Rendered { get; set; }
        public decimal? Service_Fee { get; set; }
        public decimal? Employee_Service_Rate { get; set; }
        public decimal? Fascility_Fee { get; set; }

        public decimal? Tax { get; set; }

        public DateTime Payment_Due_Date { get; set; }
        public DateTime Invoice_Date { get;  set; }

    }
}
