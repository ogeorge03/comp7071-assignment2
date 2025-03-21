using Assignment2.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Shift_Schedule
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(Employee))]
    public int SupervisorId { get; set; }
    public DateTime Start_Datetime { get; set; }
    public int Hours_Scheduled { get; set; }
    public int? Hours_Completed { get; set; }
    public string? Comments { get; set; }
    public virtual Employee Employee { get; set; }
}
