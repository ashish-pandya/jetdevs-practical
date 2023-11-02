using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JetDevs.Api.Models.DbEntities;
using JetDevs.Api.Models.ViewModels;
using JetDevs.Common.Web.ExceptionHandling;
using JetDevs.Common.Web.Security;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JetDevs.Api.Controllers
{
    /// <summary>
    /// Auth Controller
    /// </summary>
    [Produces("application/json")]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IJWTFactory _jWTFactory;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        ///  Creates instance of Auth Controller
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="jwtFactory"></param>
        /// <param name="roleManager"></param>
        public AuthController(
                UserManager<User> userManager,
                IJWTFactory jwtFactory,
                RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _jWTFactory = jwtFactory;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(JwtToken))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> LoginUser([FromBody] CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
            var customer = await _userManager.FindByNameAsync(credentials.UserName);

            if (identity == null)
            {
                return BadRequest(ErrorsHelpers.GenerateErrorResponse("Login failure", "Invalid username or Password."));
            }

            var jwt = await _jWTFactory.GenerateJwt(identity, customer.Id, "0");

            return new OkObjectResult(jwt);

        }

        /// <summary>
        /// Generates Claims Identity
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(user, password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>();
                foreach (var roleName in userRoles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    claims.AddRange(roleClaims);
                }

                return await Task.FromResult(_jWTFactory.GenerateClaimsIdentity(user.UserName, user.Id,
                user.FirstName, user.LastName, userRoles, claims));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}