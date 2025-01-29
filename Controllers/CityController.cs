using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentOffice.Data;
using RentOffice.Models;

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
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities.ToListAsync();
        }

        // GET: api/City/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // POST: api/City
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(CityRequest cityRequest)
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

            return CreatedAtAction(nameof(GetCity), new { id = city.Id }, city);
        }

        // PUT: api/City/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, CityRequest cityRequest)
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

            return NoContent();
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

            return NoContent();
        }
    }

    public class CityRequest
    {
        public string Name { get; set; }
        public string Photo { get; set; }
    }
}
