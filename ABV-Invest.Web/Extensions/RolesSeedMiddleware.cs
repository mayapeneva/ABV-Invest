namespace ABV_Invest.Web.Extensions
{
    using System;
    using System.Threading.Tasks;
    using ABV_Invest.Models;
    using Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore.Internal;

    public class RolesSeedMiddleware
    {
        private readonly RequestDelegate next;

        public RolesSeedMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<AbvInvestUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await this.RolesSeed(userManager, roleManager);
            }

            await this.next(context);
        }

        private async Task RolesSeed(UserManager<AbvInvestUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole { Name = Constants.Admin });
            await roleManager.CreateAsync(new IdentityRole { Name = Constants.User });

            var user = new AbvInvestUser
            {
                UserName = Constants.Admin,
                Email = Constants.AdminEmail,
                FullName = Constants.Admin,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var result = await userManager.CreateAsync(user, Constants.AdminPass);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Constants.Admin);
            }
        }
    }
}