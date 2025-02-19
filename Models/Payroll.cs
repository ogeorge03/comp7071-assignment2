namespace MVCSampleApp.Models
{
    public class Payroll
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public DateTime Processed_Datetime { get; set; }
        public decimal Regular_Pay { get; set; }
        public decimal Overtime_Pay { get; set; }
        public decimal Vacation_Pay { get; set; }
        public decimal Tax_Withheld { get; set; }
        public decimal Payroll_Adjustment { get; set; }
        public string? Adjustment_Reason { get; set; }
        public DateTime Pay_Period_Start_Date { get; set; }
        public DateTime Pay_Period_End_Date { get; set; }
        public string? Direct_Deposit_Number { get; set; }
        public string? Check_Number { get; set; }

    }
}
