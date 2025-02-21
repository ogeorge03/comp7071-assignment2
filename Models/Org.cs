namespace MVCSampleApp.Models
{
    public class Org
    {
        public int Id { get; set; }
        public Org? Parent_Org { get; set; }
        public string Legal_Name { get; set; }
        public StAddress? Primary_Address { get; set; }
        public StAddress? Incorporation_Address { get; set; }

        public enum Org_Type { CORP=0, CHARITY=1,NGO=2, OTHER=3}

        public Person? Legal_Contact { get; set; }
    }
}
