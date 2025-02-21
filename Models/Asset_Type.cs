namespace MVCSampleApp.Models
{
    public class Asset_Type
    {
        public int Id { get; set; }

        public Asset_Type? Parent_Asset_Type { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
