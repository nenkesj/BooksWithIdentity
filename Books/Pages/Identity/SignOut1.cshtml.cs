using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Books.Pages.Identity
{
    public class SignOutModel1 : UserPageModel
    {
        public SignOutModel1(SignInManager<IdentityUser> signMgr)
            => SignInManager = signMgr;
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            await SignInManager.SignOutAsync();
            return RedirectToPage();
        }
    }
}
