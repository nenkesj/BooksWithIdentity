﻿using Microsoft.AspNetCore.Authorization;
namespace Books.Pages.Identity.Admin
{
    //[AllowAnonymous]
    [Authorize(Roles = "Dashboard")]
    public class AdminPageModel : UserPageModel
    {
        // no methods or properties required
    }
}