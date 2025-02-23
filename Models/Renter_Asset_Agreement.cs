namespace Assignment2.Models
{
    public class Renter_Asset_Agreement
    {
        public int Id { get; set; }
        public Renter Renter { get; set; }
        public Asset Rental_Asset { get; set; }
        public decimal? Rental_Rate { get; set; }
        public DateTime Rental_Start_Date { get; set; }
        public enum RENTAL_TYPE { MONTHLY = 0, PREPAID = 1, ANNUAL = 2 }
        public DateTime? Agreement_Expiration_Date { get; set; }
        public string? Notes {  get; set; }

    }
}
