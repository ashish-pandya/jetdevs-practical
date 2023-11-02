using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using JetDevs.Api.Context;
using JetDevs.Api.Models.DbEntities;
using JetDevs.Common.Web.Security;
using JetDevs.Common.Web.Seeding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JetDevs.Api.SeedData
{
    /// <summary>
    /// Seed Database
    /// </summary>
    public class SeedDatabase : ISeed
    {
        private static readonly string[] RoleList = new string[] { Roles.ADMINISTRATOR, Roles.USER };

        /// <inheritdoc/>
        public async Task ApplyMigrations(IServiceProvider services)
        {
            using (var service = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = service.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    // Checks if there are any migrations pending
                    if (dbContext.Database.GetPendingMigrations().Any())
                    {
                        await dbContext.Database.MigrateAsync();
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task Initialize(IServiceProvider services)
        {
            using (var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>())
            {
                foreach (var role in RoleList)
                {
                    // Adds role to database if it doesn't already exist
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            using (var dbContext = services.GetService<ApplicationDbContext>())
            {
                    using (var userManager = services.GetRequiredService<UserManager<User>>())
                    {
                        if (dbContext.Users.Any())
                        {
                            // Do nothing
                        }
                        else
                        {
                            

                            var adminUser = new User
                            {
                                Email = "admin@jetdevs.com",
                                EmailConfirmed = true,
                                UserName = "admin@jetdevs.com",
                                FirstName = "Admin",
                                LastName = "User"
                            };
                            var result = await userManager.CreateAsync(adminUser, "Admin@123");
                            if (result.Succeeded)
                            {
                                var dbUser = await userManager.FindByNameAsync(adminUser.Email);
                                await userManager.AddToRoleAsync(dbUser, Roles.ADMINISTRATOR);
                            }
                            dbContext.SaveChanges();
                        }
                    }

            }
        }
    }
}
