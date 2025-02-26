namespace Assignment2.Models
{
    public class Employee_Vacation
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Vacation_Start_Date { get; set; }
        public DateTime Vacation_End_Date { get; set; }
        public bool Is_SuperVisor_Approved { get; set; }
        public Employee Supervisor { get; set; }
        public DateTime? Approval_Date { get; set; }
        public bool Is_Paid_Vacation { get; set; }
        public string Status { get; set; } = "Pending"; // New field for tracking approval status
    }
}
