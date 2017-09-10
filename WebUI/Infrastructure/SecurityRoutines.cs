using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebUI.Data.Entities;
using WebUI.Models;

namespace WebUI.Infrastructure
{
    public static class SecurityRoutines
    {
        public static async Task CreateAdminUser(UserManager<User> userManager, string username, string password)
        {
            var user = new User { UserName = username /*, Email = model.Email */ , IsApproved = true };
            var admin = await userManager.FindByNameAsync(username);
            //var r = await userManager.DeleteAsync(admin);
            //admin = await userManager.FindByNameAsync(username);
            if (admin == null)
            {
                await userManager.CreateAsync(user, password);
                await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, UserRoles.Admin, ClaimValueTypes.String));
            }
        }
    }
}