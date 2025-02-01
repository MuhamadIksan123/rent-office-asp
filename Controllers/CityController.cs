using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentOffice.Data;
using RentOffice.Models;
using RentOffice.Payload.Response;
using RentOffice.Payload.Request;

namespace RentOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/City
        [HttpGet]
        public async Task<ActionResult<CitiesResponse>> GetCities()
        {
            var cities = await _context.Cities.ToListAsync();

            var responseBody = new CitiesResponse();
            responseBody.Status = 200;
            responseBody.Message = "Success";
            responseBody.ListData = cities;

            return responseBody;
        }

        // GET: api/City/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CitiesResponse>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            var responseBody = new CitiesResponse();
            responseBody.Status = 200;
            responseBody.Message = "Success";
            responseBody.Data = city;

            return responseBody;
        }

        // POST: api/City
        [HttpPost]
        public async Task<ActionResult<City>> PostCity([FromBody] CityRequest cityRequest)
        {
            // Membuat instance City baru berdasarkan CityRequest
            var city = new City
            {
                Name = cityRequest.Name,
                Slug = cityRequest.Name.ToLower().Replace(" ", "-"),
                Photo = cityRequest.Photo,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now) // Setting CreatedAt default
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Data Berhasi Ditambahkan");
        }

        // PUT: api/City/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, [FromBody] CityRequest cityRequest)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            // Update the fields
            city.Name = cityRequest.Name;
            city.Slug = cityRequest.Name.ToLower().Replace(" ", "-");
            city.Photo = cityRequest.Photo;

            _context.Entry(city).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Data Berhasil Diubah");
        }

        // DELETE: api/City/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Ok("Data Berhasil Dihapus");
        }
    }

    public class CityRequest
    {
        public string Name { get; set; }
        public string Photo { get; set; }
    }
}
