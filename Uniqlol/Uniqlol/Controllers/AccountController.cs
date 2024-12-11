using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Uniqlol.Enums;
using Uniqlol.Extensions;
using Uniqlol.Models;
using Uniqlol.ViewModels.Auths;
using Uniqlol.ViewModels.Authsl;
using Uniqlol.Services.Abstracts;
using System.Text;

namespace Uniqlol.Controllers
{
    public class AccountController(UserManager<User> _userManager,SignInManager<User> _signInManager,RoleManager<IdentityRole> _roleManager,IEmailService _service) : Controller
    {

        public IActionResult Send()
        {
            //SmtpClient client = new SmtpClient();
            //client.Host = "smtp.gmail.com";
            //client.Port = 587;
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;
            //client.Credentials = new NetworkCredential("fidanrs-ab108@code.edu.az", "tdqj itqg rqvx jdnt");
            //MailAddress from = new MailAddress("fidanrs-ab108@code.edu.az", "Uniqlo");
            //MailAddress to = new MailAddress("fidansultanli325@gmail.com");
            //MailMessage message = new MailMessage(from, to);
            //message.Subject = "Qiymetlendirme";
            //message.Body = "<p>Salammmmm, imtahan neticen <a href='https://www.youtube.com/watch?v=3S88KB4Mkjw'> linkdedir</a>.</p>";
            //message.IsBodyHtml = true;
            //client.Send(message);
            //_service.SendAsync().Wait();
            return Ok();
        }
        private bool isAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;
        public IActionResult Register()
        {
            if(isAuthenticated) return RedirectToAction("Index","Home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid)
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
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _service.SendEmailConfirmation(user.Email, user.UserName, token);
            return Content("Email sent!");
        }

        //public async Task<IActionResult> Method()
        //{
        //    foreach (Roles item in Enum.GetValues(typeof(Roles)))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole(item.GetRole()));   
        //    }
        //    return Ok();
        //}

        public IActionResult Login()
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm,string? returnUrl = null)
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
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
            {
                if(await _userManager.IsInRoleAsync(user,"Admin"))
                {
                    return RedirectToAction("Index", new {Controller = "Dashboard", Area = "Admin"});
                }
                return RedirectToAction("Index","Home");
            }
            return LocalRedirect(returnUrl);
                
            
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> VerifyEmail(string token, string user)
        {
            var entity = await _userManager.FindByNameAsync(user);
            if (entity is null) return BadRequest();
            var result = await _userManager.ConfirmEmailAsync(entity, token.Replace(' ', '+'));
            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in result.Errors)
                {
                    sb.AppendLine(item.Description);
                }
                return Content(sb.ToString());
            }
            await _signInManager.SignInAsync(entity, true);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Reset()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Reset(ResetVM vm,string email)
        {
            if (!ModelState.IsValid) return View(vm);
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                ModelState.AddModelError("Email", "");
                return View();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

             _service.ResetPassword(user.Email, user.UserName,token);
            return Content("Email send");
        }

        public IActionResult NewPassword()
        {
            return View();
        }
    }
}
