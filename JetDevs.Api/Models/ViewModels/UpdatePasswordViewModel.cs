namespace JetDevs.Api.Models.ViewModels
{
    /// <summary>
    /// Update Password ViewModel
    /// </summary>
	public class UpdatePasswordViewModel : CredentialsViewModel
    {
        /// <summary>
        /// New Password
        /// </summary>
		public string NewPassword { get; set; }
    }
}
