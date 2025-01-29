using Microsoft.AspNetCore.Mvc;
using RentOffice.Data;
using RentOffice.Models;
using Microsoft.EntityFrameworkCore;
using RentOffice.Payload.Resource;
using RentOffice.Payload.Response;

namespace RentOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSpaceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OfficeSpaceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OfficeSpace
        [HttpGet]
        public async Task<ActionResult<OfficeSpacesResponse>> GetAllOfficeSpaces()
        {
            var officeSpaces = await _context.OfficeSpaces
                .Include(os => os.Photos)  // Mengambil semua foto terkait dengan OfficeSpace
                .Include(os => os.Benefits)  // Mengambil semua manfaat terkait dengan OfficeSpace
                .ToListAsync();  // Ambil semua data OfficeSpace

            var responseBody = new OfficeSpacesResponse();
            responseBody.Status = 200;
            responseBody.Message = "Success";
            responseBody.ListData = officeSpaces;

            var OfficeSpaceResources = officeSpaces.Select(os => new OfficeSpaceResource
            {
                Id = os.Id,
                Name = os.Name,
                Thumbnail = os.Thumbnail,
                Address = os.Address,
                Slug = os.Slug,
                IsOpen = os.IsOpen,
                IsFullBooked = os.IsFullBooked,
                Price = os.Price,
                Duration = os.Duration,
                About = os.About,
                CityId = os.CityId,
                CreatedAt = os.CreatedAt,
                Photos = os.Photos.Select(p => new PhotoDTO { Photo = p.Photo, CreatedAt = p.CreatedAt }),
                Benefits = os.Benefits.Select(b => new BenefitDTO { Name = b.Name, CreatedAt = b.CreatedAt })
            }).ToList();

            return responseBody;
        }


        // POST: api/OfficeSpace
        [HttpPost]
        public async Task<IActionResult> CreateOfficeSpace([FromBody] OfficeSpaceRequest request)
        {
            if (request.Photos.Count != 3 || request.Benefits.Count != 3)
            {
                return BadRequest("You must provide exactly 3 photos and 3 benefits.");
            }

            var city = await _context.Cities.FindAsync(request.City);
            if (city == null)
            {
                return BadRequest("City not found.");
            }

            var officeSpace = new OfficeSpace
            {
                Name = request.Name,
                Address = request.Address,
                Duration = request.Duration,
                Price = request.Price,
                Thumbnail = request.Thumbnail,
                About = request.About,
                CityId = city.Id,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Slug = request.Name.ToLower().Replace(" ", "-"),
                Photos = request.Photos.Select(p => new OfficeSpacePhoto
                {
                    Photo = p,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                }).ToList(),
                Benefits = request.Benefits.Select(b => new OfficeSpaceBenefit
                {
                    Name = b,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                }).ToList()
            };

            _context.OfficeSpaces.Add(officeSpace);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Berhasil ditambahkan");
        }

        // GET: api/OfficeSpace/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfficeSpace(int id)
        {
            var officeSpace = await _context.OfficeSpaces
                .Include(os => os.Photos)
                .Include(os => os.Benefits)
                .FirstOrDefaultAsync(os => os.Id == id);

            if (officeSpace == null)
            {
                return NotFound();
            }

            return Ok(officeSpace);
        }

        // PUT: api/OfficeSpace/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOfficeSpace(int id, [FromBody] OfficeSpaceRequest request)
        {
            if (request.Photos.Count != 3 || request.Benefits.Count != 3)
            {
                return BadRequest("You must provide exactly 3 photos and 3 benefits.");
            }

            var officeSpace = await _context.OfficeSpaces
                .Include(os => os.Photos)
                .Include(os => os.Benefits)
                .FirstOrDefaultAsync(os => os.Id == id);

            if (officeSpace == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(request.City);
            if (city == null)
            {
                return BadRequest("City not found.");
            }

            officeSpace.Name = request.Name;
            officeSpace.Address = request.Address;
            officeSpace.Duration = request.Duration;
            officeSpace.Price = request.Price;
            officeSpace.Thumbnail = request.Thumbnail;
            officeSpace.About = request.About;
            officeSpace.CityId = city.Id;
            officeSpace.Slug = request.Name.ToLower().Replace(" ", "-");

            officeSpace.Photos = request.Photos.Select(p => new OfficeSpacePhoto
            {
                Photo = p,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            }).ToList();

            officeSpace.Benefits = request.Benefits.Select(b => new OfficeSpaceBenefit
            {
                Name = b,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            }).ToList();

            await _context.SaveChangesAsync();

            return Ok(officeSpace);
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

    // Request model to match incoming data
    public class OfficeSpaceRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Duration { get; set; }
        public int Price { get; set; }
        public string Thumbnail { get; set; }
        public string About { get; set; }
        public int City { get; set; }
        public List<string> Photos { get; set; }
        public List<string> Benefits { get; set; }
    }
}
