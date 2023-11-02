using System.Threading.Tasks;

namespace JetDevs.Common.Web.Email
{
    /// <summary>
    /// EmailSender Interface
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Send email async.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendEmailAsync(string email, string subject, string message);

        /// <summary>
        /// Sends an Email using Send Grid Client
        /// </summary>
        /// <param name="sendGridKey"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task Execute(string sendGridKey, string subject, string message, string email);
    }
}
