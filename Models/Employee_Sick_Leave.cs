namespace Assignment2.Models
{
    public class Employee_Sick_Leave
    {
        public int EmployeeId { get; set; }
        public DateTime Sick_Day { get; set; }
        public string? Doctors_Note { get; set; }
    }
}
