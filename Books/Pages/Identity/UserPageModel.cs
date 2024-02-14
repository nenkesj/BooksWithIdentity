using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Books.Pages.Identity
{
    [Authorize]
    public class UserPageModel : PageModel
    {
        // no methods or properties required
    }
}