using Microsoft.Extensions.DependencyInjection;

namespace JetDevs.Common.Web.Security
{
    /// <summary>
    /// Security Policy
    /// </summary>
    public static class SecurityPolicy
    {
        /// <summary>
        /// Only Administrator role
        /// </summary>
        public const string ADMINISTRATOR_POLICY = "ADMINISTRATOR_POLICY";
        /// <summary>
        /// Only User
        /// </summary>
        public const string USER_POLICY = "USER_POLICY";
        

        /// <summary>
        /// Injects policy based authorization rules to the service.
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureAuthorizationPolicy(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(ADMINISTRATOR_POLICY, policy => policy.RequireRole(Roles.ADMINISTRATOR));
                options.AddPolicy(USER_POLICY, policy => policy.RequireRole(Roles.USER));
            });
        }
    }
}
