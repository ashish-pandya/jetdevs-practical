using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JetDevs.Common.Web.Seeding
{
    /// <summary>
    /// Database Seeder
    /// </summary>
    /// <typeparam name="T">SeedDatabase class</typeparam>
    /// <typeparam name="P">Seed caller</typeparam>
    public class Seeder<T, P> where T : ISeed, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static async Task Seed(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var seed = new T();

                    await seed.ApplyMigrations(services);
                    await seed.Initialize(services);
                }
                catch (Exception e)
                {
                    var logger = services.GetRequiredService<ILogger<P>>();
                    logger.LogError(e, "An error occurred while seeding the database.");
                    throw new ApplicationException("Database can not be seeded", e);
                }
            };
        }
    }
}
