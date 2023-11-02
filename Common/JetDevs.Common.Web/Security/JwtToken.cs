using System;

namespace JetDevs.Common.Web.Security
{
    /// <summary>
    /// Jwt token
    /// </summary>
    public class JwtToken
    {
        /// <summary>
        /// The access token string
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// The type of token
        /// </summary>
        public string token_type { get; set; } = "bearer";
        /// <summary>
        /// the duration of time the access token is granted for
        /// </summary>
        public int expires_in { get; set; }


        //public string refresh_token { get; set; }
    }
}
