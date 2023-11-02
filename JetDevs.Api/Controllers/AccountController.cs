using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JetDevs.Api.Context;
using JetDevs.Api.Models.DbEntities;
using JetDevs.Api.Models.ViewModels;
using JetDevs.Common.Web.Email;
using JetDevs.Common.Web.ExceptionHandling;
using JetDevs.Common.Web.Options;
using JetDevs.Common.Web.Security;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace JetDevs.Api.Controllers
{
    /// <summary>
    /// Account Controller
    /// </summary>
    [EnableRateLimiting("Api")]
    [Produces("application/json")]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IJWTFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtIssuerOptions;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        /// <summary>
        /// Initialize Account Controller
        /// </summary>
        /// <param name="appDbContext"></param>
        /// <param name="userManager"></param>
        /// <param name="mapper"></param>
        /// <param name="jwtFactory"></param>
        /// <param name="jwtIssuerOptions"></param>
        /// <param name="emailSender"></param>
        /// <param name="roleManager"></param>
        /// <param name="logger"></param>
        public AccountController(
                ApplicationDbContext appDbContext,
                UserManager<User> userManager,
                IMapper mapper,
                IJWTFactory jwtFactory,
                IOptions<JwtIssuerOptions> jwtIssuerOptions,
                IEmailSender emailSender,
                RoleManager<IdentityRole> roleManager,
                ILogger<AccountController> logger
            )
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _jwtFactory = jwtFactory;
            _jwtIssuerOptions = jwtIssuerOptions.Value;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Creates a user account and sends confirmation email
        /// </summary>
        /// <param name="registrationUser"></param>
        /// <returns>Success Object</returns>
        [Route("CreateUser")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]RegistrationUserViewModel registrationUser)
        {
            var userIdentity = _mapper.Map<User>(registrationUser);

            var result = await _userManager.CreateAsync(userIdentity, registrationUser.Password);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(ErrorsHelpers.GenerateErrorResponse(result));
            }

            _logger.LogInformation("User added successfully.");

            foreach (var role in registrationUser.Roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    var user = await _userManager.FindByEmailAsync(registrationUser.Email);
                    await _userManager.DeleteAsync(user);

                    return BadRequest(ErrorsHelpers.GenerateErrorResponse("Bad Role", "Role does not exist. Please try again with the correct roles."));
                }

                var addUserToRole = await _userManager.AddToRoleAsync(userIdentity, role);

                _logger.LogInformation("User added to roles {roles}.", registrationUser.Roles.ToString());
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userIdentity);

            var callbackUrl = Url.Action(
                "ConfirmEmail", "Account",
                new { user_id = userIdentity, code },
                protocol: Request.Scheme
            );

            //await _emailSender.SendEmailAsync(
            //    registrationUser.Email,
            //    "Confirm your email!",
            //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>"
            //);

            _logger.LogInformation("User confirmation email sent.");

            return Ok();
        }

        /// <summary>
        /// Uses HttpContext User to find a user by their Id.
        /// </summary>
        /// <returns>Non-sensative user information</returns>
        [Authorize]
        [Route("GetCurrentUser")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            _logger.LogInformation("Getting current user information.");

            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user != null)
            {
                _logger.LogInformation("User found. Returning user ID: {id} to requester.", user.Id);

                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.AccessFailedCount
                });
            }
            else
            {
                _logger.LogError("Unable to find user. Claims may be missing from token request.");
                return BadRequest(ErrorsHelpers.GenerateErrorResponse("User not found!", "The user requested does not exist."));
            }
        }

        /// <summary>
        /// Updates a user account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [Route("UpdateUser")]
        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserViewModel model)
        {
            _logger.LogInformation("Updating user ID: {0}", model.UserName);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Unable to update user with ID: {0}. Model is invalid.", model.UserName);
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.UserName);

            var newFirstName = string.IsNullOrEmpty(model.NewFirstName) ? user.FirstName : model.NewFirstName;
            var newLastName = string.IsNullOrEmpty(model.NewLastName) ? user.LastName : model.NewLastName;

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                _logger.LogInformation("User verified. Attempting to update user information for user ID: {0}", model.UserName);

                user.FirstName = newFirstName;
                user.LastName = newLastName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully updated user ID: {0}", user.Id);
                    return Ok(new
                    {
                        Success = "User updated successfully."
                    });
                }
                else
                {
                    _logger.LogError("Cannot update user ID {0}. Error from User Manager {1}", model.UserName, result.Errors);
                    return BadRequest(ErrorsHelpers.GenerateErrorResponse(result));
                }
            }
            else
            {
                _logger.LogError("Unable to change user ID: {0} credentials. Username or password invalid.", model.UserName);
                return BadRequest(ErrorsHelpers.GenerateErrorResponse("Wrong password", "Unable to update. Username or password invalid."));
            }
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Ok object.</returns>
        [Authorize(Policy = SecurityPolicy.ADMINISTRATOR_POLICY)]
        [Authorize]
        [Route("RemoveUser")]
        [HttpPost]
        public async Task<IActionResult> RemoveUser([FromBody]RemoveUserViewModel model)
        {
            _logger.LogInformation("Attempting to remove user ID: {0}.", model.UserId);

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("User verified. Attempting to update user information for user ID: {0}", model.UserId);
                return BadRequest(ModelState);
            }

            var userIdentity = _appDbContext.Users.FirstOrDefault(u => u.Id == model.UserId);

            if (await _userManager.CheckPasswordAsync(userIdentity, model.Password))
            {
                _logger.LogInformation("User verified. Attempting to update user information for user ID: {0}", model.UserId);

                var result = await _userManager.DeleteAsync(userIdentity);

                if (result.Succeeded)
                {
                    return Ok(new { Success = "Successfully removed user" });
                }
                else return BadRequest(ErrorsHelpers.GenerateErrorResponse(result));
            }
            else return BadRequest(ErrorsHelpers.GenerateErrorResponse("Wrong password", "Unable to delete. Username or password invalid."));

        }

        /// <summary>
        /// Confirms user confirmation email using code generated during registration
        /// </summary>
        /// <param name="email">User Email</param>
        /// <param name="code">Code generated from user creation</param>
        /// <returns></returns>
        [Route("ConfirmEmail")]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string code)
        {
            _logger.LogInformation("Attempting to validate user with validation code: {code}", code);

            if (email == null || code == null)
            {
                _logger.LogWarning("No user id or confirmation code provided.");
                return BadRequest(ErrorsHelpers.GenerateErrorResponse("Bad email or code", "Missing UserID or Code in request"));
            }

            var userIdentity = _appDbContext.Users.FirstOrDefault(u => u.Email == email);

            if (await _userManager.IsEmailConfirmedAsync(userIdentity))
            {
                _logger.LogWarning("User already confirmed. Returning success.");

                return Ok(new
                {
                    Success = "User is already confirmed."
                });
            }

            var result = await _userManager.ConfirmEmailAsync(userIdentity, code);

            if (result.Succeeded)
            {
                _logger.LogInformation("User confirmed. Returning success.");

                return Ok(userIdentity.Email);
            }
            else
            {
                _logger.LogWarning("Unable to confirm user. Failed with error: {error}", result.Errors);
                return new BadRequestObjectResult(ErrorsHelpers.GenerateErrorResponse(result));
            }
        }

        /// <summary>
        /// Sends user an email resetting their password. Generated a token using the ASP.NET Identity library.  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Content Invalid");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            _logger.LogInformation("Sending reset password email for user ID: {0}", user.Id);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                _logger.LogInformation("User null or email not yet confirmed. Unable to send email");
                return Ok(new
                {
                    Success = "A password reset email has been sent to the registered email address."
                });
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { username = user.UserName, code },
                protocol: Request.Scheme
            );

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset your password",
                "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>"
            );

            _logger.LogInformation("Successfully sent 'Forgot Password' email to user ID: {0}.", user.Id);
            return Ok(new
            {
                Success = "A password reset email has been sent to the registered email address."
            });
        }

        /// <summary>
        /// Resets a user password after receiving a confirmation code and providing a new password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string code, [FromBody]ResetPasswordViewModel model)
        {
            if (email == null || code == null)
            {
                return BadRequest(ErrorsHelpers.GenerateErrorResponse("Bad email or code", "Missing UserID or Code in request"));
            }
            else if (model.NewPassword == null)
            {
                return BadRequest(ErrorsHelpers.GenerateErrorResponse("No password", "No password was given"));
            }

            var user = await _userManager.FindByEmailAsync(email);

            var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Success = "Successfully updated user password"
                });
            }
            else return BadRequest(ErrorsHelpers.GenerateErrorResponse(result));
        }

        /// <summary>
        /// Updates the users password. Requires correct old password to succeed.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [Route("UpdatePassword")]
        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody]UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _appDbContext.Users.FirstOrDefault(u => u.UserName == model.UserName);

            if (await _userManager.CheckPasswordAsync(userIdentity, model.Password))
            {
                var result = await _userManager.ChangePasswordAsync(userIdentity, model.Password, model.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        Success = "Password updated successfully."
                    });
                }
                else return BadRequest(ErrorsHelpers.GenerateErrorResponse(result));
            }
            else return BadRequest(ErrorsHelpers.GenerateErrorResponse("Wrong password", "Password Invalid"));
        }
    }
}