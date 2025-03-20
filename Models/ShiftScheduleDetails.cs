namespace Assignment2.Models
{
    public class ShiftScheduleDetails
    {
        public int Id { get; set; }
        public DateTime Start_Datetime { get; set; }
        public int Hours_Scheduled { get; set; }
        public int? Hours_Completed { get; set; }
        public string? Comments { get; set; }
        public string? Supervisor { get; set; }
    }
}
