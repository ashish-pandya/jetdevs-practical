using JetDevs.Api.Context;
using JetDevs.Api.Models.DbEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JetDevs.Api
{
    /// <summary>
    /// Password policy builder options
    /// </summary>
    public class BuilderOptions
    {
        /// <summary>
        /// Gets Identity Builder options
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IdentityBuilder GetBuilderOptions(IServiceCollection services)
        {
            var builderOptions = services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            });

            builderOptions = new IdentityBuilder(builderOptions.UserType, typeof(IdentityRole), builderOptions.Services);
            builderOptions.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            return builderOptions;
        }
    }
}
