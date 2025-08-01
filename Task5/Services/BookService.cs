using Task5.Models;
namespace Task5.Services
{
    public class BookService
    {
        private readonly TextGenerator _textGenerator;
        private readonly ImageGenerator _imageGenerator;

        private int bias = 0;
        public int seed { get; set; } = 0;
        private double likesDef = 5;
        private double reviewsDef = 5.0;

        public BookService(TextGenerator TextGenerator,ImageGenerator ImageGenerator)
        {
            _textGenerator = TextGenerator;
            _imageGenerator = ImageGenerator;
        }
        public Book Create(int Id)
        {
            string title = _textGenerator.Title();
            string author = _textGenerator.GetAuthor();
            return new Book
            {
                Id = Id,
                Isbn = _textGenerator.ISBN(),
                Title = title,
                Author = author,
                Publisher = _textGenerator.PublisherNameWithDate(),
                Image = _imageGenerator.ImageWithCanvas(title, author),
                NumberOfLikes = _textGenerator.GetLikes(likesDef),
                AuthorRev = _textGenerator.GetSetOfReviewsAuthor(reviewsDef),
                TextRev = _textGenerator.GetSetOfReviewsText(reviewsDef)
            };
        }
        public List<Book> CreateTwentyBooks(List<Book> ListOfBooks)
        {
            _textGenerator.ChangeSeed(seed, bias);
            _imageGenerator.ChangeSeed(seed, bias);
            for (int i=bias+1;i<=bias+20;++i)
            {
                ListOfBooks.Add(Create(i));
            }
            bias += 20;
            return ListOfBooks;
        }
        public List<Book> RecreateTwentyBooks(List<Book> ListOfBooks)
        {
            ListOfBooks = new List<Book>();
            bias = 0;
            CreateTwentyBooks(ListOfBooks);
            return ListOfBooks;
        }
        public void ChangeLocale(string locale)
        {
            _textGenerator.ChangeLocale(locale);
        }

        public List<Book> ChangeToRandomSeed()
        {
            int value = _textGenerator.GenerateSeed();
            seed = value;
            return RecreateTwentyBooks(new List<Book>());
        }

        public List<Book> CreateWithSeed(int seedVal)
        {
            seed = seedVal;
            return RecreateTwentyBooks(new List<Book>());
        }

        public List<Book> RecreateAdditionalInfo(double likes,double reviews,List<Book> oldList)
        {
            if (likes != likesDef) RecreateLikes(likes, oldList);
            if (reviews != reviewsDef) RecreateReviews(reviews, oldList);
            return oldList;
        }
        public void RecreateLikes(double likes,List<Book> oldList)
        {
            likesDef = likes;
            for(int i=0;i<oldList.Count;++i)
            {
                oldList[i].NumberOfLikes = _textGenerator.GetLikes(likes);
            }
        }
        public void RecreateReviews(double reviews, List<Book> oldList)
        {
            reviewsDef = reviews;
            for (int i = 0; i < oldList.Count; ++i)
            {
                oldList[i].TextRev = _textGenerator.GetSetOfReviewsText(reviewsDef);
                oldList[i].AuthorRev = _textGenerator.GetSetOfReviewsAuthor(reviewsDef);
            }
        }
    }
}
