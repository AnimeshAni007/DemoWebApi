using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            //SeedInitialUsers();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user == null) return Unauthorized("Invalid credentials");

            // Generate JWT Token payload tokens
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) // Injects "Admin" or "User" into the signed payload
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Token valid for 2 hours
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString, username = user.Username, role = user.Role });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromQuery] string username, [FromQuery] string password, [FromQuery] string role = "User")
        {
            // 1. Check if the username is already taken
            var userExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (userExists)
            {
                return BadRequest("Username is already taken.");
            }

            // 2. Create the new user object (Hardcoded to 'User' role by default from the query string)
            var newUser = new UserItem
            {
                Username = username,
                Password = password, // Note: In production, you should hash this password!
                Role = role
            };

            // 3. Save the new user to your SQL database
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }


        private void SeedInitialUsers()
        {
            if (!_context.Users.Any())
            {
                _context.Users.Add(new UserItem { Username = "admin", Password = "password123", Role = "Admin" });
                _context.Users.Add(new UserItem { Username = "user", Password = "password123", Role = "User" });
                _context.SaveChanges();
            }
        }
    }
}
