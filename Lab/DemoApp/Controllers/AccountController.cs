using ITIEntities.Data;
using ITIEntities.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemoApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITIContext _db;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public AccountController(ITIContext db)
        {
            _db = db;
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View();
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View();
            }

            var roleName = _db.Roles.Where(r => r.Id == user.RoleId).Select(r => r.Name).FirstOrDefault();

            // build the user claims out of the things that define the user's identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Then we also add the role as a claim because it's so important for authorization, and we will be using it a lot in the application, so it's better to have it as a claim rather than having to query the database every time we need to check the user's role
            if (!string.IsNullOrEmpty(roleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            // The identity of the user is built up of the set of claims that we have defined above, and the authentication scheme (cookie in this case)
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // using this identity we create a principal which is the object that will be used to represent the user in the application, it contains the identity and the claims
            var principal = new ClaimsPrincipal(identity);

            // Finally we sign in the user by passing the principal to the SignInAsync method, this will create the authentication cookie and set it in the response
            // So a cookie will be sent to be stored on the client side, and this cookie will be sent to the server with each request
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // After successful login, we redirect the user to the returnUrl if it's provided and is a local URL, otherwise we redirect to the home page
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
