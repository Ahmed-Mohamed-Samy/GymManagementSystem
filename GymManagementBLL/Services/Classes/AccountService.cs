using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AccountViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<ApplicationUser?> LoginAsync(AccountViewModel accountViewModel)
        {
            var user = await _userManager.FindByEmailAsync(accountViewModel.Email);

            if (user == null) return null;

            var IsPasswordVaild = await _userManager.CheckPasswordAsync(user, accountViewModel.Password);

            return IsPasswordVaild ? user : null;
        }
    }
}
