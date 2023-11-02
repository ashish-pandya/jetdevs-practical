using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using JetDevs.Common.Web.Options;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JetDevs.Common.Web.Security
{

    /// <summary>
    /// Authentication Configuration
    /// </summary>
    public static class Authentication
    {

        /// <summary>
        /// Injects authentication attributes to the service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns>Extension of the IServiceCollection</returns>
        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtAppSettingsOptions = configuration.GetSection(nameof(JwtIssuerOptions));
            var _secret = configuration["SigningKey"];
            SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secret));

            // Parameters used when validating and generating tokens
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)],
                ValidateAudience = true,
                ValidAudience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
                options.TokenValidationParameters = tokenValidationParameters;
            });

            // Global schemes
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Global bearer options for Jwt
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.ClaimsIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
                jwtOptions.TokenValidationParameters = tokenValidationParameters;
                jwtOptions.SaveToken = true;
            });

            services.ConfigureApplicationCookie(cookieOptions =>
            {
                cookieOptions.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, cookieOptions.Events.OnRedirectToAccessDenied);
                cookieOptions.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, cookieOptions.Events.OnRedirectToLogin);
            });

            return services;
        }

        /// <summary>
        /// Prevents redirects on requests defined in ConfigureApplicationCookies() 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="existingRedirector"></param>
        /// <returns></returns>
        static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
            context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = (int)statusCode;
                    return Task.CompletedTask;
                }
                return existingRedirector(context);
            };

    }
}
