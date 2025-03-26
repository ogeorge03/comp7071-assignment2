namespace Assignment2.Models
{
    public class Certification
    {
        public int Id { get; set; }
        public string Certification_Description { get; set; }
        public string? Certification_Authority { get; set; }

        public string? Certification_Number { get; set; }

        public ICollection<Employee_Certification> Employee_Certifications { get; set; }
    }
}
