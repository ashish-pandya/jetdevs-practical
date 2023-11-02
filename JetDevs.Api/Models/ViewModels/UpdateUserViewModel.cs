namespace JetDevs.Api.Models.ViewModels
{
	/// <summary>
	/// Update User ViewModel
	/// </summary>
	public class UpdateUserViewModel : CredentialsViewModel
    {
		/// <summary>
		/// New First Name
		/// </summary>
		public string NewFirstName { get; set; }
		/// <summary>
		/// New Last Name
		/// </summary>
		public string NewLastName { get; set; }
    }
}
