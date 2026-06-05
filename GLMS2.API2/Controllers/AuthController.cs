using GLMS2.API2.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GLMS2.API2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var configuredUsername = _configuration["DemoUser:Username"];
            var configuredPassword = _configuration["DemoUser:Password"];

            if (request.Username != configuredUsername ||
                request.Password != configuredPassword)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            var token = GenerateJwtToken(request.Username);

            return Ok(token);
        }

        private LoginResponseDto GenerateJwtToken(string username)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is missing.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Jwt:Issuer is missing.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("Jwt:Audience is missing.");

            var expiresInMinutes = int.TryParse(
                _configuration["Jwt:ExpiresInMinutes"],
                out var minutes)
                ? minutes
                : 120;

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("role", "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt
            };
        }
    }
}