using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JetDevs.Common.Web.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JetDevs.Common.Web.Security
{
    /// <summary>
    /// JWT Factory
    /// </summary>
    public class JWTFactory : IJWTFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger<JWTFactory> _logger;

        /// <summary>
        /// Creates instance of JWT Factory
        /// </summary>
        /// <param name="jwtOptions"></param>
        /// <param name="logger"></param>
        public JWTFactory(
                IOptions<JwtIssuerOptions> jwtOptions,
                ILogger<JWTFactory> logger
            )
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        /// <summary>
        /// If missing any mandatory configuration settings. Throws error and prevents application from running.
        /// </summary>
        /// <param name="options"></param>
        private void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <summary>
        /// Generates encoded JWT using claims generated from GenerateClaimsIdentity
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="companyId"></param>
        /// <param name="identity"></param>
        /// <returns>Signed JWT</returns>
        public async Task<string> GenerateEncodedToken(string userName, string companyId, ClaimsIdentity identity)
        {
            _logger.LogInformation("Creating token for user {0}", userName);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtCustomClaimNames.COMPANY_ID, companyId),
            };

            foreach (Claim claim in identity.Claims)
            {
                claims.Add(claim);
            }

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

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
        public ClaimsIdentity GenerateClaimsIdentity(string userName,
            string id,
            string firstName,
            string lastName,
            IList<string> roles,
            List<Claim> claims)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, firstName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, lastName));

            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            foreach (var claim in claims)
            {
                identity.AddClaim(claim);
            }

            return identity;
        }

        /// <summary>
        /// Validates token using signature.
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <param name="roleToValidate"></param>
        /// <returns>Returns role list to requester. This is then used for verifying policies in other services.</returns>
        public string ValidateTokenAndRole(string jwtToken, string roleToValidate)
        {
            _logger.LogInformation("Attempting to validate role from token. Message received via MQ");
            try
            {
                SecurityToken validatedToken;

                new JwtSecurityTokenHandler().ValidateToken(jwtToken, _jwtOptions.TokenValidationParameters, out validatedToken);

                if (validatedToken != null)
                {
                    JwtSecurityToken token = new JwtSecurityToken(jwtToken);

                    foreach (Claim claim in token.Claims)
                    {
                        if (claim.Type == ClaimTypes.Role)
                        {
                            if (claim.Value == roleToValidate)
                            {
                                _logger.LogInformation("Token authenticated and role found. Returning success message");
                                return "Authorized";
                            }
                            else
                            {
                                _logger.LogError("Unable to locate roles in token. Returning failure message");
                                return "No roles in token";
                            }
                        }
                        else
                        {
                            _logger.LogError("Unable to locate roles in token. Returning failure message");
                            return "No roles in token";
                        }
                    }
                }
                else
                {
                    _logger.LogError("Unable to validate security token. Returning failure message");
                    return "Token cannot be validated";
                }

                _logger.LogError("Unable to validate security token. Returning failure message");
                return "Token cannot be validated";
            }
            catch (Exception e)
            {
                _logger.LogError("Exception in validating token. Error was: ", e);
                return e.Message;
            }
        }

        /// <inheritdoc/>
        public async Task<JwtToken> GenerateJwt(ClaimsIdentity identity, string userId, string companyId)
        {
            return new JwtToken
            {
                access_token = await GenerateEncodedToken(userId, companyId, identity),
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };
        }

        /// <summary>
        /// Convert to Unix datetime format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);
    }
}
