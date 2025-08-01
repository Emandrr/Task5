using Task5.Models;
namespace Task5.Contracts
{
    public record TranslateBooksRequest
    {
        public List<Book> Books { get; set; }
        public string Language { get; set; }
    }
}
