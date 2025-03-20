namespace Assignment2.Models
{
    public class Customer : Person
    {
        public int Id { get; set; }
        public string? Customer_Notes { get; set; }
        public string? Payment_Information { get; set; }
        public Contact_Information? Contact_Information { get; set; }
        public string Email { get; set; } 
    }
}
