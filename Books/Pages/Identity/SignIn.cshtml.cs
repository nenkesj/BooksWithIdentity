using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
namespace Books.Pages.Identity
{
    [AllowAnonymous]
    public class SignInModel : UserPageModel
    {
        public SignInModel(SignInManager<IdentityUser> signMgr,
                UserManager<IdentityUser> usrMgr)
        {
            SignInManager = signMgr;
            UserManager = usrMgr;
        }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public UserManager<IdentityUser> UserManager { get; set; }
        [Required]
        [EmailAddress]
        [BindProperty]
        public string Email { get; set; }
        [Required]
        [BindProperty]
        public string Password { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                SignInResult result = await
 SignInManager.PasswordSignInAsync(Email,
                    Password, true, true);
                if (result.Succeeded)
                {
                    TempData["message"] = "Signin Successful - " + ReturnUrl;
                    return Redirect(ReturnUrl ?? "/");
                }
                else if (result.IsLockedOut)
                {
                    TempData["message"] = "Account Locked";
                }
                else if (result.IsNotAllowed)
                {
                    IdentityUser user = await UserManager.FindByEmailAsync(Email);
                    if (user != null &&
                           !await UserManager.IsEmailConfirmedAsync(user))
                    {
                        return RedirectToPage("SignUpConfirm");
                    }
                    TempData["message"] = "Sign In Not Allowed";
                }
                else if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("SignInTwoFactor", new { ReturnUrl });
                }
                else
                {
                    TempData["message"] = "Sign In Failed";
                }
            }
            else
            {
                TempData["message"] = "Model isnt valid";
            }
            return Page();
        }
        public IActionResult OnPostExternalAsync(string provider)
        {
            string callbackUrl = Url.Page("SignIn", "Callback", new { ReturnUrl });
            AuthenticationProperties props =
               SignInManager.ConfigureExternalAuthenticationProperties(
                   provider, callbackUrl);
            return new ChallengeResult(provider, props);
        }
        public async Task<IActionResult> OnGetCallbackAsync()
        {
            ExternalLoginInfo info = await SignInManager.GetExternalLoginInfoAsync();
            SignInResult result = await SignInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded)
            {
                return Redirect(WebUtility.UrlDecode(ReturnUrl ?? "/"));
            }
            else if (result.IsLockedOut)
            {
                TempData["message"] = "Account Locked";
            }
            else if (result.IsNotAllowed)
            {
                TempData["message"] = "Sign In Not Allowed";
            }
            else
            {
                TempData["message"] = "Sign In Failed";
            }
            return RedirectToPage();
        }
    }
}