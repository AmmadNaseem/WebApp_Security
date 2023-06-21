using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_WithIdentity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // now we use the expension mehtod that is in .AspNetCore.Authentication and give Authentication Scheme name
            await HttpContext.SignOutAsync("MyCookieAuth");

            return RedirectToPage("/Index");
        }
    }
}
