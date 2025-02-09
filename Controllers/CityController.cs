using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentOffice.Data;
using RentOffice.Models;
using RentOffice.Payload.Response;
using RentOffice.Payload.Request;
using RentOffice.Services;

namespace RentOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;

        public CityController(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
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
        public async Task<ActionResult<City>> PostCity([FromForm] CityRequest cityRequest)
        {

            string photoPath = await _fileService.SaveFileAsync(cityRequest.PhotoFile, "cities");

            // Membuat instance City baru berdasarkan CityRequest
            var city = new City
            {
                Name = cityRequest.Name,
                Slug = cityRequest.Name.ToLower().Replace(" ", "-"),
                Photo = photoPath,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now) // Setting CreatedAt default
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Data Berhasi Ditambahkan");
        }

        // PUT: api/City/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, [FromForm] CityRequest cityRequest)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound("Kota tidak ditemukan.");
            }

            // Perbarui nama dan slug
            city.Name = cityRequest.Name;
            city.Slug = cityRequest.Name.ToLower().Replace(" ", "-");

            // Jika ada file baru yang diunggah, ganti foto lama
            if (cityRequest.PhotoFile != null)
            {
                // Hapus foto lama jika ada
                if (!string.IsNullOrEmpty(city.Photo))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", city.Photo.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Simpan foto baru
                city.Photo = await _fileService.SaveFileAsync(cityRequest.PhotoFile, "cities");
            }

            _context.Entry(city).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Data Berhasil Diubah", city });
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
}
