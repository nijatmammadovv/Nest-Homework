using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest_Homework_Partial.Models;
using Nest_Homework_Partial.ViewModels.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_Homework_Partial.Controllers
{
    public class AuthController : Controller
    {
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signInManager { get; }
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser appUser = new AppUser
            {
                Name = registerVM.Firstname,
                Surname = registerVM.Lastname,
                UserName = registerVM.Username,
                Email = registerVM.Email
            };
            IdentityResult result = await _userManager.CreateAsync(appUser, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            await _signInManager.SignInAsync(appUser, true);
            return RedirectToAction("Index", "Home");
        }
            public async Task<IActionResult> SignOut()
            {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Register));
            }
    }
}
