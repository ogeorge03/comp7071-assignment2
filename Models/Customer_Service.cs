namespace MVCSampleApp.Models
{
    public class Customer_Service
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public decimal Hourly_Rate { get; set; }

        public Certification? Certification_Required { get; set; }

    }
}
