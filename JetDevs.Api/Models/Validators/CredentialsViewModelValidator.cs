using FluentValidation;
using JetDevs.Api.Models.ViewModels;

namespace JetDevs.Api.Models.Validators
{
    /// <summary>
    /// Credentials ViewModel Validator
    /// </summary>
	public class CredentialsViewModelValidator : AbstractValidator<CredentialsViewModel>
    {
        /// <summary>
        /// Creates instance of Credentials ViewModel Validator
        /// </summary>
        public CredentialsViewModelValidator()
        {
            RuleFor(vm => vm.UserName).NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(vm => vm.UserName).EmailAddress().WithMessage("Username must be valid email");
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(vm => vm.Password).Length(8, 16).WithMessage("Password must be between 8 and 16 characters");
        }
    }
}
