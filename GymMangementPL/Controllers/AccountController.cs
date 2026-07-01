using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AccountViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAccountService accountService , SignInManager<ApplicationUser> signInManager)
        {
            _accountService = accountService;
            _signInManager = signInManager;
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(AccountViewModel accountView)
        {
            if(!ModelState.IsValid) return View(accountView);
            

            var user = await _accountService.LoginAsync(accountView);

            if(user is null)
            {
                ModelState.AddModelError("InvalidLogin", "InValid Email or Password");
                return View(accountView);
            }

            var Result = await _signInManager.PasswordSignInAsync(user, accountView.Password, accountView.RememberMe, false);
            
            if(Result.IsNotAllowed)
                ModelState.AddModelError("InvalidLogin", "Your Account Not Allowed");
            if(Result.IsLockedOut)
                ModelState.AddModelError("InvalidLogin", "Your Account Locked out");
            if (Result.Succeeded)
                return RedirectToAction("Index", "Home");

            return View(accountView);


        }


        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }


        public ActionResult AccessDenied()
        {
            return View(); 
        }
    }
}
