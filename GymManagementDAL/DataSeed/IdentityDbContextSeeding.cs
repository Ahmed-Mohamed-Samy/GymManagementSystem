using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.DataSeed
{
    public static class IdentityDbContextSeeding
    {
        public static async Task<bool> SeedDataAsync(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            try
            {
                var HasUsers = await userManager.Users.AnyAsync();

                var HasRoles = await roleManager.Roles.AnyAsync();

                if (HasUsers && HasRoles) return false;
                
                if(!HasRoles)
                {
                    var Roles = new List<IdentityRole>()
                    {
                        new() {Name = "SuperAdmin"},
                        new() {Name = "Admin"}
                    };

                    foreach(var Role in Roles)
                    {
                        var IsRoleExist = await roleManager.RoleExistsAsync(Role.Name!);


                        if (!IsRoleExist)
                            await roleManager.CreateAsync(Role);
                        

                    }

                }

                if(!HasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Ahmed",
                        LastName = "Samy",
                        UserName = "AhmedSamy",
                        Email = "ahmedsamy1ami@gmail.com",
                        PhoneNumber = "01016334658"
                    };

                    await userManager.CreateAsync(MainAdmin,"P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");


                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Tarek",
                        UserName = "MohamedTarek",
                        Email = "MohamedTarek@example.com",
                        PhoneNumber = "01015151515"
                    };

                    await userManager.CreateAsync(Admin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");

                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed Failed : {ex}");
                return false;
            }
        }
    }
}
