using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace JetDevs.Common.Web.Email
{
    /// <summary>
    /// Send emails from the portal
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// Creates a new instance of the sender
        /// </summary>
        /// <param name="emailSettings"></param>
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        ///<inheritdoc/>
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(_emailSettings.SendGridKey, subject, message, email);
        }

        ///<inheritdoc/>
        public Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_emailSettings.FromAddress, _emailSettings.FromName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}
