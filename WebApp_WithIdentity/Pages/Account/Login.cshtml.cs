using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebApp_WithIdentity.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty] // this will give a felling of 2 way binding.
        public Crendential Credential { get; set; }
        //Modle is used to communicate between front and back 
        public void OnGet()
        {
            // when page will reload then get hendler will execute.
           // this.Credential = new Crendential { userName="ammad"};
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            //======verify the credentials
            if (Credential.userName=="admin" && Credential.Password=="password")
            {
                //========creating the security context: we will add claim in key pairs:
                var claims = new List<Claim> { 
                    new Claim(ClaimTypes.Name,"admin"),
                    new Claim(ClaimTypes.Email,"admin@gmail.com"),
                    new Claim("Department","HR"),
                    new Claim("EmploymentDate","2023-02-01")
                     // new Claim("Admin",true),
                };

                //=========== we need to add these claims in identity. and give in it authentication type
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                
                //======once we have identity then needs for claims principles.
                ClaimsPrincipal claimsPrincipal=new ClaimsPrincipal(identity);

                //=========before signing the cookie we have another veriable authentication properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                // now we have generated the claims principle next step is encrypt and serialized security context.

                //=======here we also specify the authentication scheme and principle.
                await HttpContext.SignInAsync("MyCookieAuth",claimsPrincipal,authProperties); //this will serialized the claims principle
                                                                               //into the strings and encrypt that string and save that 
                                                                               // cookie and http context object.

                return RedirectToPage("/Index");
 
            }

            // if verification failed then again return page
            return Page();
        }

        public class Crendential
        {
            [Required]
            [Display(Name ="User Name")]
            public string userName { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember Me")]
            public bool RememberMe { get; set; }
        }
    }
}
