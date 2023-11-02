using System.Collections.Generic;

namespace JetDevs.Api.Models.ViewModels
{
    /// <summary>
    /// Registration User ViewModel
    /// </summary>
    public class RegistrationUserViewModel : BaseUserViewModel
    {
        /// <summary>
        /// Roles
        /// </summary>
		public List<string> Roles { get; set; }
    }
}
