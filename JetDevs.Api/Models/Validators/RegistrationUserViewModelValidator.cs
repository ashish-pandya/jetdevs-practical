using FluentValidation;
using JetDevs.Api.Models.ViewModels;
namespace JetDevs.Api.Models.Validators
{
    /// <summary>
    /// Registration User ViewModel Validator
    /// </summary>
	public class RegistrationUserViewModelValidator : AbstractValidator<RegistrationUserViewModel>
    {
        /// <summary>
        /// Creates instance of Registration User ViewModel Validator
        /// </summary>
        public RegistrationUserViewModelValidator()
        {
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(vm => vm.Email).EmailAddress().WithMessage("Email is not in a valid format");
            RuleFor(vm => vm.Password).Length(8, 16).WithMessage("Password must be between 8 and 16 characters");
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(vm => vm.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(vm => vm.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(vm => vm.Roles).NotEmpty().WithMessage("A user role must be provided.");
        }
    }
}
