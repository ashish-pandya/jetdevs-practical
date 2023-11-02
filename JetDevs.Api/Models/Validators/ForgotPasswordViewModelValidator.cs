using FluentValidation;
using JetDevs.Api.Models.ViewModels;

namespace JetDevs.Api.Models.Validators
{
	/// <summary>
	/// Forgot Password ViewModel Validator
	/// </summary>
	public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
	{
		/// <summary>
		/// Creates instance of Forgot Password ViewModel Validator
		/// </summary>
		public ForgotPasswordViewModelValidator()
		{
			RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email is required.");
		}
	}
}
