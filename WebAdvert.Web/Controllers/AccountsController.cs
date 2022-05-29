using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public async Task<IActionResult> SignUp()
        {
            var model = new SignUpViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);

                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists.");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

                if (createdUser.Succeeded)
                    return RedirectToAction("Confirm");
            }

            return View();
        }

        public async Task<IActionResult> Confirm(ConfirmViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found.");
                    return View(model);
                }

                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
                else
                {
                    foreach (var item in result.Errors)
                        ModelState.AddModelError(item.Code, item.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(LoginViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    false);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("LoginError", "Email and password do not match");
            }

            return View("Login", model);
        }

        public async Task<IActionResult> Signout()
        {
            if (User.Identity.IsAuthenticated) await _signInManager.SignOutAsync().ConfigureAwait(false);

            return RedirectToAction("Login");
        }
    }
}
