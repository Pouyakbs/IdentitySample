using IdentitySample.Data;
using IdentitySample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext context;
        public AdminController(RoleManager<IdentityRole> roleManager , UserManager<IdentityUser> UserManager , ApplicationDbContext context)
        {
            this.roleManager = roleManager;
            userManager = UserManager;
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UsersInfo()
        {
            List<IdentityUser> users = userManager.Users.ToList();
            List<IdentityUserRole<string>> Roles = context.UserRoles.ToList();
            List<IdentityRole> Role = roleManager.Roles.ToList();
            UserViewModel model = new UserViewModel()
            {
                Users = users,
                UserRoles = Roles,
                Roles = Role,
            };
            return View(model);
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            IdentityUser result = await userManager.FindByIdAsync(id);
            await userManager.DeleteAsync(result);
            return RedirectToAction("UsersInfo");
        }
        public async Task<IActionResult> EditUser(string id)
        {
            IdentityUser result = await userManager.FindByIdAsync(id);
            var role = await userManager.GetRolesAsync(result);
            UserViewModel model = new UserViewModel()
            {
                user = result,
                EditRoles = role,
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(IdentityUser user , string roleName , bool LockedOut)
        {
            IdentityUser model = await userManager.FindByIdAsync(user.Id);
            model.LockoutEnabled = LockedOut;
            var role = await userManager.GetRolesAsync(model);
            foreach (var item in role)
            {
                await userManager.RemoveFromRoleAsync(model, item);
            }
            model.Email = user.Email;
            model.UserName = user.Email;
            model.NormalizedEmail = user.Email.ToUpperInvariant();
            model.NormalizedUserName = user.Email.ToUpperInvariant();
            IdentityResult result = await userManager.UpdateAsync(model);
            await userManager.AddToRoleAsync(model, roleName);
            return RedirectToAction("UsersInfo", result);
        }
        public IActionResult RolesInfo()
        {
            List<IdentityRole> roles = roleManager.Roles.ToList();
            UserViewModel model = new UserViewModel()
            {
                Roles = roles,
            };
            return View(model);
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole result = await roleManager.FindByIdAsync(id);
            var roleName = await roleManager.GetRoleNameAsync(result);
            var userId = await userManager.GetUsersInRoleAsync(roleName);
            foreach (var item in userId)
            {
                await userManager.RemoveFromRoleAsync(item, roleName);
                await userManager.AddToRoleAsync(item, "User");
            }
            await roleManager.DeleteAsync(result);
            return RedirectToAction("index");
        }
        public async Task<IActionResult> EditRole(string id)
        {
            IdentityRole result = await roleManager.FindByIdAsync(id);
            UserViewModel model = new UserViewModel()
            {
                role = result,
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(IdentityRole role)
        {
            IdentityRole model = await roleManager.FindByIdAsync(role.Id);
            model.Name = role.Name;
            model.NormalizedName = role.Name.ToUpperInvariant();
            IdentityResult result = await roleManager.UpdateAsync(model);
            return RedirectToAction("RolesInfo" , result);
        }
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserViewModel userViewModel , string roleName)
        {
            IdentityUser user = new IdentityUser()
            {
                Email = userViewModel.Email,
                UserName = userViewModel.Email,
            };
            IdentityResult result = await userManager.CreateAsync(user, userViewModel.Password);
            await userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return RedirectToAction("index", "home");
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("Identity", item.Description);
            }
            return View();
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = roleName
                };
                await roleManager.CreateAsync(role);
            }
            return RedirectToAction("index");
        }
    }
}
