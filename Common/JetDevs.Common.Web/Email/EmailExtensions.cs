using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JetDevs.Common.Web.Email
{
    /// <summary>
    /// Contains email configuration extension
    /// </summary>
    public static class EmailExtensions
    {
        /// <summary>
        /// Configures the eaail sender.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection(nameof(EmailSettings));
            services.Configure<EmailSettings>(options =>
            {
                options.FromAddress = emailSettings.GetValue<String>(nameof(EmailSettings.FromAddress), EmailSettings.DEFAULT_FROM_ADDRESS);
                options.FromName = emailSettings.GetValue<String>(nameof(EmailSettings.FromName), EmailSettings.DEFAULT_FROM_NAME);
            });
            services.AddSingleton<IEmailSender, EmailSender>();
            return services;
        }
    }
}
