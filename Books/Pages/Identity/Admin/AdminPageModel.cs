using Microsoft.AspNetCore.Authorization;

namespace Books.Pages.Identity.Admin
{
    [AllowAnonymous]
    public class AdminPageModel : UserPageModel
    {
        // no methods or properties required
    }
}
