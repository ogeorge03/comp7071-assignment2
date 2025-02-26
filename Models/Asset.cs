namespace Assignment2.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public Asset? Parent_Asset { get; set; }
        public int Asset_TypeId { get; set; }
        public string Name { get; set; }
        public string? Color { get; set; }
        public decimal? Rental_Rate { get; set; }
        public decimal? Lease_Price { get; set; }
        public decimal? Book_Value { get; set; }
        public decimal? Depreciation {  get; set; }
        public StAddress? Address { get; set; }
        public string? Description { get; set; }
        public DateTime Begin_Date { get; set; }


    }
}
