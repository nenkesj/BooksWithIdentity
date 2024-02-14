using HowTo_DBLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Books.Pages
{
    public class EditModel : PageModel
    {
        public EditModel(HowToDBContext ctx) => DbContext = ctx;
        public HowToDBContext DbContext { get; set; }
        public Node Node { get; set; }
        public void OnGet(long id)
        {
            Node = DbContext.Find<Node>(id) ?? new Node();
        }
        public IActionResult OnPost([Bind(Prefix = "Node")] Node n)
        {
            DbContext.Update(n);
            DbContext.SaveChanges();
            return RedirectToPage("Admin");
        }
    }
}
