using System;

namespace JetDevs.Common.Web.Email
{
    /// <summary>
    /// EmailSettings
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Default From address.
        /// </summary>
        public const String DEFAULT_FROM_ADDRESS = "support@JetDevs.com";
        /// <summary>
        /// Default from name.
        /// </summary>
        public const String DEFAULT_FROM_NAME = "JetDevs Portal";
        /// <summary>
        /// From address. Dafault value is DEFAUL_FROM_ADDRESS 
        /// </summary>
        public String FromAddress { get; set; }
        /// <summary>
        /// From name. Default value is DEFAUL_FROM_NAME
        /// </summary>
        public String FromName { get; set; }
        /// <summary>
        /// Sendgrid key to send emails
        /// </summary>
        public string SendGridKey { get; set; }
    }
}
