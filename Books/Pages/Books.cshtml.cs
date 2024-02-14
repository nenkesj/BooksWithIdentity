using HowTo_DBLibrary;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Books.Pages
{
    public class BooksModel : PageModel
    {
        public BooksModel(HowToDBContext ctx) => DbContext = ctx;
        public HowToDBContext DbContext { get; set; }
    }
}
