using HowTo_DBLibrary;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Books.Pages
{
    public class AdminModel : PageModel
    {
        public AdminModel(HowToDBContext ctx) => DbContext = ctx;
        public HowToDBContext DbContext { get; set; }
    }
}
