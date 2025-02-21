namespace MVCSampleApp.Models
{
    public class Shift_Schedule
    {
        public Employee Employee { get; set; }
        public Employee Supervisor { get; set; }
        public DateTime Start_Datetime { get; set; }
        public int Hours_Scheduled { get; set; }
        public int? Hours_Completed { get; set; }
        public string? Comments { get; set; }

    }
}
