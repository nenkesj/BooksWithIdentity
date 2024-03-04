using Books.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace Books.Pages.Identity
{
    [AllowAnonymous]
    public class UserAccountCompleteModel : UserPageModel
    {
        public UserAccountCompleteModel(UserManager<IdentityUser> usrMgr,
                TokenUrlEncoderService tokenUrlEncoder)
        {
            UserManager = usrMgr;
            TokenUrlEncoder = tokenUrlEncoder;
        }
        public UserManager<IdentityUser> UserManager { get; set; }
        public TokenUrlEncoderService TokenUrlEncoder { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }
        [BindProperty]
        [Required]
        public string Password { get; set; }
        [BindProperty]
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await UserManager.FindByEmailAsync(Email);
                string decodedToken = TokenUrlEncoder.DecodeToken(Token);
                IdentityResult result = await UserManager.ResetPasswordAsync(user,
                    decodedToken, Password);
                if (result.Process(ModelState))
                {
                    return RedirectToPage("Admin/Dashboard", new { });
                }
            }
            return Page();
        }
    }
}
