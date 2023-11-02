using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JetDevs.Common.Web.Security
{
    /// <summary>
    /// Interface for JWT Factory
    /// </summary>
    public interface IJWTFactory
    {
        /// <summary>
        /// Generates encoded JWT using claims generated from GenerateClaimsIdentity
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="companyId"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        /// <returns>Signed JWT</returns>
        Task<string> GenerateEncodedToken(string userName, string companyId, ClaimsIdentity identity);

        /// <summary>
        /// Generates ClaimsIdentity from user credentials taken from database.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="id"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="roles"></param>
        /// <param name="claims"></param>
        /// <returns>List of claims. Currently only returns Role ClaimTypes</returns>
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id, string firstName, string lastName, IList<string> roles, List<Claim> claims);

        /// <summary>
        /// Validates token using signature.
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <param name="roleToValidate"></param>
        /// <returns>Returns role list to requester. This is then used for verifying policies in other services.</returns>
        string ValidateTokenAndRole(string jwtToken, string roleToValidate);

        /// <summary>
        /// Generates signed JWT
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<JwtToken> GenerateJwt(ClaimsIdentity identity, string userId, string companyId);

    }

}
