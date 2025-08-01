using Microsoft.AspNetCore.Mvc;
using Bogus;
using Task5.Models;
using Task5.Services;
using Task5.Contracts;
//Randomizer.Seed = WHAC2006(seed)
public class HomeController : Controller
{

    private readonly BookService _bookService;

    public HomeController(BookService bookService)
    {
        _bookService = bookService;
    }
    
    public IActionResult Index()
    {
        var items = _bookService.ChangeToRandomSeed();
        ViewBag.Seed = _bookService.seed;
        return View(items);
    }

    [HttpPost]
    public JsonResult GetMoreItems([FromBody] List<Book> currentBooks)
    {
        var items = _bookService.CreateTwentyBooks(new List<Book>());
        return Json(items);
    }
    [HttpPost]
    public JsonResult TranslateBooks([FromBody] TranslateBooksRequest translateBooks)
    {
        _bookService.ChangeLocale(translateBooks.Language);
        var items = _bookService.RecreateTwentyBooks(new List<Book>());
        return Json(items);
    }
    [HttpGet]
    public JsonResult GenerateBooksWithSeed(int seed)
    {
        _bookService.seed = seed;

        var items = _bookService.CreateWithSeed(seed);
        return Json(items);
    }
    [HttpGet]
    public JsonResult GenerateBooksWithoutSeed()
    {
        var items = _bookService.ChangeToRandomSeed();
        ViewBag.Seed = _bookService.seed;
        var viewModel = new BookWithSeedViewModel
         {
             Seed = _bookService.seed,
             Books = items
         };

        return Json(viewModel);
    }
    [HttpPost]
    public JsonResult UpdateFilterData([FromBody] AdditionalInfoRequest info)
    {
        var items = _bookService.RecreateAdditionalInfo(info.likes,info.reviews,info.Books);
        return Json(items);
    }
}

