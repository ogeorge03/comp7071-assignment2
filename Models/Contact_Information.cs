namespace Assignment2.Models
{
    public class Contact_Information
    {
        public int Id { get; set; }
        public Person Person { get; set; }
        public enum CONTACT_TYPE { PHONE=0, EMAIL=1, OTHER=2}

        public CONTACT_TYPE Contact_Type { get; set; }

        public string Contact_Info { get; set; }

    }
}
