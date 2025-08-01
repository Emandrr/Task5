namespace Task5.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Isbn { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public string? Image { get; set; }
        public int NumberOfLikes { get; set; }
        public List<string> AuthorRev { get; set; } = new List<string>();
        public List<string> TextRev { get; set; } = new List<string>();
    }
}
