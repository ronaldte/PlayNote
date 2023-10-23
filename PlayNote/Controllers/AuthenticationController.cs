using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlayNote.Controllers
{
    /// <summary>
    /// Provides simple JWT token generation for authentication.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        /// <summary>
        /// Creates new instance of authentication controller.
        /// </summary>
        /// <param name="configuration">Configuration containing properties used for JWT.</param>
        /// <exception cref="ArgumentNullException">JWT requieres configuration data.</exception>
        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Represents model for login data.
        /// </summary>
        public class AuthenticationModel
        {
            /// <summary>
            /// Username for login.
            /// </summary>
            public string? Username { get; set; }
            
            /// <summary>
            /// Loging password for given username.
            /// </summary>
            public string? Password { get; set; }
        }

        /// <summary>
        /// Represents database user.
        /// </summary>
        private class PlayNoteUser
        {
            /// <summary>
            /// User Id
            /// </summary>
            public int UserId { get; set; }
            
            /// <summary>
            /// User name for user.
            /// </summary>
            public string UserName { get; set; }
            
            /// <summary>
            /// First name of the user.
            /// </summary>
            public string FirstName { get; set; }
            
            /// <summary>
            /// Last (family) name of the user.
            /// </summary>
            public string LastName { get; set; }

            /// <summary>
            /// Creates new instance of the user.
            /// </summary>
            /// <param name="userId">Id of the new user.</param>
            /// <param name="userName">Username of the new user.</param>
            /// <param name="firstName">First name of the new user.</param>
            /// <param name="lastName">Last name of the new user.</param>
            public PlayNoteUser(int userId, string userName, string firstName, string lastName)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
            }
        }

        /// <summary>
        /// Fake method fetching user entity from database based on authentication.
        /// </summary>
        /// <param name="model">User model with login details.</param>
        /// <returns>Application user if login detail are correct, null otherwise.</returns>
        private PlayNoteUser? ValidateUserCredentials(AuthenticationModel model)
        {
            return new PlayNoteUser(1, "playnoteTester123", "John", "Doe");
        }

        /// <summary>
        /// Authenticates user and generates new auth bearer JWT token.
        /// </summary>
        /// <param name="authenticationModel">Login details</param>
        /// <returns>JWT token if successfull otherwise Unauthorized.</returns>
        /// <response code="200">Returns JWT token for given user.</response>
        /// <response code="401">Incorrect login details.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public ActionResult<string> Authenticate(AuthenticationModel authenticationModel)
        {
            var user = ValidateUserCredentials(authenticationModel);

            if (user == null)
            {
                return Unauthorized();
            }

            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretKey"]!));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim("sub", user.UserId.ToString()),
                new Claim("given-name", user.FirstName),
                new Claim("family-name", user.LastName),
                //new Claim("role", "admin")
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                signingCredentials
            );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }
    }
}
