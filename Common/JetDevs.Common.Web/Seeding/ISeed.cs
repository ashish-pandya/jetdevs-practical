using System;
using System.Threading.Tasks;

namespace JetDevs.Common.Web.Seeding
{
    /// <summary>
    /// Seed Interface
    /// </summary>
    public interface ISeed
    {
        /// <summary>
        /// Apply Migrations
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        Task ApplyMigrations(IServiceProvider services);

        /// <summary>
        /// Intializes the Data
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        Task Initialize(IServiceProvider services);
    }
}
