namespace Assignment2.Models
{
    public class EmployeeDetails
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string StreetName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Employer { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTime EmploymentStartDate { get; set; }
        public DateTime? EmploymentTerminationDate { get; set; }
        public decimal PayRateAmount { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
