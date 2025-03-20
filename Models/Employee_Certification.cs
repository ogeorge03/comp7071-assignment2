namespace Assignment2.Models
{
    public class Employee_Certification
    {
        public int EmployeeId { get; set; }

        public int CertificationId { get; set; }

        public DateTime? Certification_Valid_Until {  get; set; }
        public DateTime? Certification_Received_Date { get; set; }



    }
}
