namespace MediaRemove.Models
{
    public class UnusedMedia
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Id { get; set; }
        public string Source { get; set; }
        public string MediaType { get; set; }
        public string BackofficeLink { get; set; } //contains direct BO edit link
    }
}
