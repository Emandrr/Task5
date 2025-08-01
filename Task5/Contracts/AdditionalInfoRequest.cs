using Task5.Models;
namespace Task5.Contracts
{
    public record AdditionalInfoRequest
    {
        public double likes { get; set; }
        public double reviews { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
        
    }
}
