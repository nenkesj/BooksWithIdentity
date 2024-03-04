using Microsoft.AspNetCore.Mvc;
namespace Books.Pages.Identity
{
    public class SignInCodesWarningModel : UserPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/";
    }
}
