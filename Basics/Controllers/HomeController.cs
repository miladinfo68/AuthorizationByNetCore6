using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [Authorize] public IActionResult Secret() => View();

        //this is accessable only once we've defined the MobilePhone claim on Authenticate method
        [Authorize(Policy = "MobileNumberPolicy")] public IActionResult SecretByPolicy() => View();

        //this is accessable only once we've defined the Admin Role claim on Authenticate method
        [Authorize(Roles ="Admin")] public IActionResult SecretByRole() => View();
        [Authorize(Policy ="Admin")] public IActionResult SecretByAdminPolicy() => View("SecretByRole");

        public IActionResult Authenticate()
        {
            List<Claim> userIdentityClaims = new(){
                new Claim(ClaimTypes.Name,"TestUser") ,
                new Claim(ClaimTypes.Email,"TestUser@gmail.com") ,
                //if we comment these both claims [SecretByPolicy] and [SecretByRole] not ganna work
                new Claim(ClaimTypes.MobilePhone,"093555555") ,
                new Claim(ClaimTypes.Role,"Admin") ,
            };

            List<Claim> userLicenseClaims = new(){
                new Claim(ClaimTypes.Name,"TestUser License") ,
                new Claim("DrivingLicense","A+") ,
            };

            ClaimsIdentity userInfoIdentity = new(userIdentityClaims, "TestUserInfo Identity");
            ClaimsIdentity userLicenseIdentity = new(userLicenseClaims, "TestUserLicense Identity");

            ClaimsPrincipal userPrincipals = new(new[] { userInfoIdentity, userLicenseIdentity });

            HttpContext.SignInAsync(userPrincipals);

            return RedirectToAction("Index");
        }

    }
}