using IdentityServer4.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IdentityServer4.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _userSignIn;
        private readonly IEmailService _emailService;

        public HomeController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> userSignIn,
            IEmailService emailService)
        {
            _userManager = userManager;
            _userSignIn = userSignIn;
            _emailService = emailService;
        }

        public IActionResult Index() => View();

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user != null)
            {
                //when config.SignIn.RequireConfirmedEmail=true in builder.Services.AddIdentity<IdentityUser, IdentityRole>
                //user can't login till verify the sent email!

                var result = await _userSignIn.PasswordSignInAsync(user, userDto.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(VerifyEmail), "home", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());
                await _emailService.SendAsync(user.Email, "Verify Login Email", $"<a href=\"{link}\">Verify Email - {DateTime.Now.ToLongDateString()}</a>", true);

                return RedirectToAction("EmailVerification");
            }
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {
            IdentityUser user = new()
            {
                UserName = userDto.UserName,
                Email = userDto.Email
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            return View();
        }


        public IActionResult EmailVerification() => View();

        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return View("Error");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return RedirectToAction("Login");
            return View();
        }




        public async Task<IActionResult> Logout()
        {
            await _userSignIn.SignOutAsync();
            return RedirectToAction("Login");
        }


        [Authorize] public IActionResult Secret() => View();
    }
}