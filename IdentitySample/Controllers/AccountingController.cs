using IdentitySample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySample.Controllers
{
    public class AccountingController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountingController(UserManager<IdentityUser> UserManager, SignInManager<IdentityUser> SignInManager, RoleManager<IdentityRole> roleManager)
        {
            userManager = UserManager;
            signInManager = SignInManager;
            this.roleManager = roleManager;
        }
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {

                Microsoft.AspNetCore.Identity.SignInResult result =
                await signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, false);
                await addClaim(login.Email);
                //IdentityUser model = await userManager.FindByIdAsync(login.user.Id);
                //if (model.LockoutEnabled == true)
                //{
                //    return Content("Your Account is Disabled");
                //}

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(login.ReturnUrl))
                    {
                        return Redirect(login.ReturnUrl);
                    }
                    return RedirectToAction("index", "home");

                }
                ViewBag.login = "نام کاربری یا رمز عبور اشتباه است";
            }
            return View(login);
        }



        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser()
                {
                    Email = userViewModel.Email,
                    UserName = userViewModel.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, userViewModel.Password);
                await userManager.AddToRoleAsync(user, "User");

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("Identity", item.Description);
                }

            }
            return View(userViewModel);

        }
        public async Task addClaim(string email)
        {
            IdentityUser user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await userManager.AddClaimsAsync(user, new Claim[]
            {
                   new Claim("Ban Status",user.LockoutEnabled.ToString()),
            });
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

    }
}
