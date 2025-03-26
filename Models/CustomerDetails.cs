namespace Assignment2.Models
{
    public class CustomerDetails
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime Date_Of_Birth { get; set; }
        public string? Customer_Notes { get; set; }
        public string? Payment_Information { get; set; }
    }
}
