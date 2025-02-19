namespace MVCSampleApp.Models
{
    public class Renter : Person
    {
        public bool Passed_Credit_Check { get; set; }
        public Person? Emergency_Contact { get; set; }
        public Person? Family_Doctor { get; set; }

    }
}
