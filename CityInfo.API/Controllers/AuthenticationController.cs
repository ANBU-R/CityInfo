using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Define a class to represent the request body for authentication
        public class AuthenticationRequestBody
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authReqBody)
        {
            // For demo purposes, assume if the authReqBody contains 
            // username and password, it is always valid 
            if (authReqBody.Username == null || authReqBody.Password == null)
            {
                // If either username or password is null, return 401 Unauthorized
                return Unauthorized();
            }

            // Creating a new SymmetricSecurityKey instance using the secret key from configuration
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));

            // Creating SigningCredentials for JWT token validation using HMAC-SHA256 algorithm
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            // Create claims for the JWT token
            var claimsForToken = new List<Claim>();
            // Dummy values for demonstration purposes
            claimsForToken.Add(new Claim("sub", "Dummy UserId"));
            claimsForToken.Add(new Claim("given_name", "Dummy Firstname"));
            claimsForToken.Add(new Claim("family_name", "Dummy Secondname"));
            claimsForToken.Add(new Claim("city", "Dummy city"));

            // Create a JWT token with specified parameters
            var jwtToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(2),
                signingCredentials
                );


            // Generate the JWT token string
            var jwtTokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // Return the generated JWT token string
            return Ok(jwtTokenString);


        }
    }
}
