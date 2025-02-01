using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentOffice.Data;
using RentOffice.Models;
using RentOffice.Payload.Response;
using RentOffice.Services;

namespace RentOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public AuthController(JwtService jwtService, ApplicationDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        // Fitur Registrasi
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validasi format email (mengharuskan adanya '@')
            if (!request.Email.Contains("@"))
            {
                return BadRequest(new { message = "Email must contain '@' symbol." });
            }

            // Validasi role
            if (request.Role != "Admin" && request.Role != "Manager" && request.Role != "User")
            {
                return BadRequest(new { message = "Invalid role. Allowed roles are: Admin, Manager, User." });
            }

            // Cek apakah email sudah ada
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (existingUser != null)
            {
                return Conflict(new { message = "Email already exists." });
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Simpan user baru
            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = hashedPassword,
                Role = request.Role,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // Fitur Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users
                .Where(u => u.Email.ToLower() == request.Email.ToLower())
                .FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                var response = new AuthResponse()
                {
                    Status = 401,
                    Message = "Email or password is incorrect."
                };
                return Unauthorized(response);
            }

            var roles = new List<string> { user.Role };  // Ambil role dari user
            var token = _jwtService.GenerateToken(user.Email, roles);
            return Ok(new { token });
        }

        // Fitur Logout
        [HttpPost("logout")]
        [Authorize(Roles = "Admin")]
        public IActionResult Logout()
        {
            // Biasanya logout dilakukan dengan menghapus token dari client-side
            return Ok(new { message = "Logged out successfully." });
        }
    }

    // DTO untuk registrasi
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // Admin, Manager, User
    }

    // DTO untuk login
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}