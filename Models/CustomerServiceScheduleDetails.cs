namespace Assignment2.Models
{
    public class CustomerServiceScheduleDetails
    {
        public int id {  get; set; }
        public string? ServiceName { get; set; }
        public string? FacilityName {  get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public DateTime? RenderedDateTime { get; set; }
        public decimal? ServiceFee { get; set; }
        public int CustomerSatisfactionRating { get; set; }
        public string? Comments { get; set; }
    }
}
