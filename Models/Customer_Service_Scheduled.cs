namespace Assignment2.Models
{
    public class Customer_Service_Scheduled
    {

        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Customer_Service Customer_Service { get; set; }
        public Facility Facility { get; set; }
        public DateTime Scheduled_DateTime { get; set; }
        public DateTime? Rendered_DateTime { get; set; }
        public decimal? Service_Fee { get; set; }
        public int Customer_Satisfaction_Rating { get; set; }
        public string? Comments {  get; set; }
    }
}
