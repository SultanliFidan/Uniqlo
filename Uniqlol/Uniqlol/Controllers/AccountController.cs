﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uniqlol.Enums;
using Uniqlol.Extensions;
using Uniqlol.Models;
using Uniqlol.ViewModels.Auths;
using Uniqlol.ViewModels.Authsl;

namespace Uniqlol.Controllers
{
    public class AccountController(UserManager<User> _userManager,SignInManager<User> _signInManager,RoleManager<IdentityRole> _roleManager) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            User user = new User
            {
                Fullname = vm.Fullname,
                Email = vm.Email,
                UserName = vm.Username
            };
           var result = await _userManager.CreateAsync(user,vm.Password);
            if(!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View();
            }
           var roleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
            if (!roleResult.Succeeded)
            {
                foreach (var err in roleResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View();
            }

            return View();
        }

        public async Task<IActionResult> Method()
        {
            foreach (Roles item in Enum.GetValues(typeof(Roles)))
            {
                await _roleManager.CreateAsync(new IdentityRole(item.GetRole()));   
            }
            return Ok();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm,string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View();
            User? user = null;
            if(vm.UsernameOrEmail.Contains("@"))
                user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail); 
            else
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Username or passqord is wrong!");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user,vm.Password,vm.RememberMe,true);    
            if(!result.Succeeded)
            {
                if(result.IsLockedOut)
                {
                    ModelState.AddModelError("","Wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if(result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Username or passqord is wrong!");
                }
                return View();
            }
            if(string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index","Home");
            return LocalRedirect(returnUrl);
                
            
        }
    }
}
