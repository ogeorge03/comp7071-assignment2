namespace Assignment2.Models
{
    public class CustomerServiceScheduleDetails
    {
        public int id {  get; set; }
        public string? Description { get; set; }
        public decimal HourlyRate { get; set; }
        public string? CertificationAuthority { get; set; }
        public string? CertificationDescription { get; set; }
        public DateTime ScheduledDateTime { get; set; }
    }
}
