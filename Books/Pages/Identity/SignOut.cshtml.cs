using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Books.Pages.Identity
{
    [AllowAnonymous]
    public class SignOutModel : UserPageModel
    {
        public SignOutModel(SignInManager<IdentityUser> signMgr)
            => SignInManager = signMgr;
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            await SignInManager.SignOutAsync();
            return RedirectToPage();
        }
    }
}

