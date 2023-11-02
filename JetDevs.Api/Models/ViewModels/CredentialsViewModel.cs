namespace JetDevs.Api.Models.ViewModels
{
    /// <summary>
    /// Credentials Model - Used to submit login request
    /// </summary>
    public class CredentialsViewModel
    {
        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}
