using System.ComponentModel.DataAnnotations;

namespace JetDevs.Api.Models.ViewModels
{
    /// <summary>
    /// Base User ViewModel
    /// </summary>
	public class BaseUserViewModel
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }
    }
}
