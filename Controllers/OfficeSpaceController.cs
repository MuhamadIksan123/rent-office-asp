using Microsoft.AspNetCore.Mvc;
using RentOffice.Data;
using RentOffice.Models;
using Microsoft.EntityFrameworkCore;
using RentOffice.Payload.Response;
using RentOffice.Payload.Request;
using RentOffice.Services;

namespace RentOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSpaceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;

        public OfficeSpaceController(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/OfficeSpace
        [HttpGet]
        public async Task<ActionResult<OfficeSpacesResponse>> GetAllOfficeSpaces()
        {
            var officeSpaces = await _context.OfficeSpaces
                .Include(os => os.Photos)  // Mengambil semua foto terkait dengan OfficeSpace
                .Include(os => os.Benefits)
                .Include(os => os.City) // Mengambil semua manfaat terkait dengan OfficeSpace
                .ToListAsync();  // Ambil semua data OfficeSpace

            var responseBody = new OfficeSpacesResponse();
            responseBody.Status = 200;
            responseBody.Message = "Success";
            responseBody.ListData = officeSpaces;

            return responseBody;
        }


        // POST: api/OfficeSpace
        [HttpPost]
        public async Task<IActionResult> CreateOfficeSpace([FromForm] OfficeSpaceRequest request)
        {
            if (request.Photos.Count != 3 || request.Benefits.Count != 3)
            {
                return BadRequest("Anda harus mengunggah tepat 3 foto dan memberikan 3 manfaat.");
            }

            var city = await _context.Cities.FindAsync(request.CityId);
            if (city == null)
            {
                return BadRequest("Kota tidak ditemukan.");
            }

            // Simpan Thumbnail
            string thumbnailPath = await _fileService.SaveFileAsync(request.Thumbnail, "office_thumbnails");

            // Simpan Foto
            var photoPaths = new List<OfficeSpacePhoto>();
            foreach (var photo in request.Photos)
            {
                string photoPath = await _fileService.SaveFileAsync(photo, "office_photos");
                photoPaths.Add(new OfficeSpacePhoto
                {
                    Photo = photoPath,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                });
            }

            var officeSpace = new OfficeSpace
            {
                Name = request.Name,
                Address = request.Address,
                Duration = request.Duration,
                Price = request.Price,
                Thumbnail = thumbnailPath,
                About = request.About,
                CityId = city.Id,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Slug = request.Name.ToLower().Replace(" ", "-"),
                Photos = photoPaths,
                Benefits = request.Benefits.Select(b => new OfficeSpaceBenefit
                {
                    Name = b,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                }).ToList()
            };

            _context.OfficeSpaces.Add(officeSpace);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Berhasil ditambahkan", officeSpace });
        }


        // GET: api/OfficeSpace/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OfficeSpacesResponse>> GetOfficeSpace(int id)
        {
            var officeSpace = await _context.OfficeSpaces
                .Include(os => os.Photos)
                .Include(os => os.Benefits)
                .FirstOrDefaultAsync(os => os.Id == id);

            if (officeSpace == null)
            {
                return NotFound();
            }

            var responseBody = new OfficeSpacesResponse();
            responseBody.Status = 200;
            responseBody.Message = "Success";
            responseBody.Data = officeSpace;

            return responseBody;
        }

        // PUT: api/OfficeSpace/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOfficeSpace(int id, [FromForm] OfficeSpaceRequest request, List<IFormFile> photos, IFormFile thumbnail)
        {
            var officeSpace = await _context.OfficeSpaces
                .Include(os => os.Photos)
                .Include(os => os.Benefits)
                .FirstOrDefaultAsync(os => os.Id == id);

            if (officeSpace == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(request.CityId);
            if (city == null)
            {
                return BadRequest("City not found.");
            }

            officeSpace.Name = request.Name;
            officeSpace.Address = request.Address;
            officeSpace.Duration = request.Duration;
            officeSpace.Price = request.Price;
            officeSpace.About = request.About;
            officeSpace.CityId = city.Id;
            officeSpace.Slug = request.Name.ToLower().Replace(" ", "-");

            if (thumbnail != null)
            {
                officeSpace.Thumbnail = await _fileService.SaveFileAsync(thumbnail, "thumbnails");
            }

            if (photos != null && photos.Count > 0)
            {
                officeSpace.Photos = photos.Select(p => new OfficeSpacePhoto
                {
                    Photo = _fileService.SaveFileAsync(p, "photos").Result,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                }).ToList();
            }

            if (request.Benefits != null && request.Benefits.Count > 0)
            {
                officeSpace.Benefits = request.Benefits.Select(b => new OfficeSpaceBenefit
                {
                    Name = b,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                }).ToList();
            }

            await _context.SaveChangesAsync();

            return Ok("Data berhasil diubah");
        }

        // DELETE: api/OfficeSpace/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOfficeSpace(int id)
        {
            var officeSpace = await _context.OfficeSpaces
                .Include(os => os.Photos)
                .Include(os => os.Benefits)
                .FirstOrDefaultAsync(os => os.Id == id);

            if (officeSpace == null)
            {
                return NotFound();
            }

            _context.OfficeSpaces.Remove(officeSpace);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
