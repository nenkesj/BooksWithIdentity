using HowTo_DBLibrary;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Books.Pages
{
    public class LandingModel : PageModel
    {
        public LandingModel(HowToDBContext ctx) => DbContext = ctx;
        public HowToDBContext DbContext { get; set; }
    }
}
