namespace Assignment2.Models
{
    public class Employee_Certification
    {
        public Employee Employee { get; set; }

        public Certification Certification { get; set; }

        public DateTime? Certification_Valid_Until {  get; set; }
        public DateTime? Certification_Received_Date { get; set; }



    }
}
