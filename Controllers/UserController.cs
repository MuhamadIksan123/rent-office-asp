using Microsoft.AspNetCore.Mvc;
using RentOffice.Data;
using RentOffice.Models;
using RentOffice.Payload.Request;
using RentOffice.Payload.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace RentOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<UsersResponse>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            var responseBody = new UsersResponse();
            responseBody.Status = 200;
            responseBody.Message = "Success";
            responseBody.ListData = users;

            return responseBody;
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UsersResponse>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { Status = 404, Message = "User not found" });
            }

            var responseBody = new UsersResponse
            {
                Status = 200,
                Message = "Success",
                Data = user
            };

            return responseBody;
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request)
        {
            // Cek apakah email sudah digunakan
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { Status = 400, Message = "Email already exists" });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = hashedPassword,
                Role = request.Role,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Data Berhasil Ditambahkan");
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Status = 404, Message = "User not found" });
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.Role = request.Role;

            // Jika password baru dikirim, lakukan hashing sebelum disimpan
            if (!string.IsNullOrEmpty(request.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();
            return Ok("Data Berhasil Diubah");
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Status = 404, Message = "User not found" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Data Berhasil Didelete");
        }
    }
}
