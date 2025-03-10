namespace Assignment2.Models
{
    public class CustomerServiceDetails
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public decimal Hourly_Rate { get; set; }
        public int Certification_Required { get; set; }

        public int Certification_Id { get; set; }
        public string? Certification_Description { get; set; }
        public string? Certification_Authority { get; set; }
        public string? Certification_Number { get; set; }


    }
}
