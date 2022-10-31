using DTO.Models.Authen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashStoreClient.DataAccess;
using System.Security.Claims;

namespace NashStoreClient.Controllers
{
    public class AuthController : Controller
    {
        private IData _data;

        public AuthController(IData data)
        {
            _data = data;
        }

        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel input, string returnUrl)
        {
            returnUrl = HttpContext.Request.Query["returnUrl"];
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                var token = await _data.LoginAsync(input);
                if (token == null)
                {
                    ModelState.AddModelError("Error", "Your account is not valid. Try again!");
                    return View();
                }
                else
                {
                    var claims = new List<Claim>();
                    foreach (var role in token.UserInfo.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    claims.Add(new Claim("token", "Bearer " + token.TokenString));
                    claims.Add(new Claim("expiration", token.Expiration.ToString()));
                    claims.Add(new Claim("userid", token.UserInfo.Id.ToString()));
                    claims.Add(new Claim("username", token.UserInfo.UserName));
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrinciple);

                    TempData["Message"] = "Login success";
                    return RedirectToAction("Index", "Products");
                }
            }
            catch (Refit.ApiException e)
            {
                var errorList = await e.GetContentAsAsync<Dictionary<string, string>>();
                TempData["Error"] = errorList?.First().Value;
                return View();
            }
            
        }
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Products");
        }

        public async Task<ActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register([Bind("Username, Email, Password")]RegisterModel registerModel, string confirmPassword)
        {
            if(registerModel.Password != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Confirm password field must match with password field");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _data.RegisterAsync(registerModel);
                }
                catch (Refit.ApiException e)
                {
                    var errorList = await e.GetContentAsAsync<Dictionary<string, string>>();
                    TempData["Error"] = errorList?.FirstOrDefault(x => x.Key == "message").Value;
                    return View();
                }
            }
            else
            {
                return View();
            }
            TempData["Message"] = "Register success";
            return RedirectToAction("Login", "Auth");
        }
    }
}
