using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> signInManager;
        private readonly UserManager<CognitoUser> userManager;
        private readonly CognitoUserPool pool;

        public AccountsController(
            SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager,
            CognitoUserPool pool)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.pool = pool;
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
                var user = pool.GetUser(model.Email);

                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists.");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                var createdUser = await userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

                if (createdUser.Succeeded)
                {
                    RedirectToAction("Confim");
                }
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
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found.");
                    return View(model);
                }

                var result = await (userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
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
                var result = await signInManager.PasswordSignInAsync(
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
            if (User.Identity.IsAuthenticated) await signInManager.SignOutAsync().ConfigureAwait(false);
            return RedirectToAction("Login");
        }
    }
}
