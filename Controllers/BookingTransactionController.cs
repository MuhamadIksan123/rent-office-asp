using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentOffice.Data;
using RentOffice.Models;
using RentOffice.Payload.Request;
using RentOffice.Payload.Response;

namespace RentOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingTransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingTransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BookingTransactionResponse>> GetBookingTransactions()
        {
            var transactions = await _context.BookingTransactions
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.Photos)
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.Benefits)
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.City)
                .ToListAsync();


            var responseBody = new BookingTransactionResponse();
            responseBody.Status = 200;
            responseBody.Message = "Success";
            responseBody.ListData = transactions;

            return responseBody;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookingTransactionAdmin([FromBody] BookingTransactionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var officeSpace = await _context.OfficeSpaces.FindAsync(request.OfficeSpaceId);
            if (officeSpace == null)
            {
                return NotFound("Office space not found.");
            }

            DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Now);
            var bookingTransaction = new BookingTransaction
            {
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                BookingTrxId = BookingTransaction.GenerateUniqueTrxId(_context),
                Slug = request.Name.ToLower().Replace(" ", "-"),
                IsPaid = request.IsPaid,
                Duration = officeSpace.Duration,
                StartedAt = request.StartedAt,
                EndedAt = request.StartedAt.AddDays(officeSpace.Duration),
                TotalAmount = request.TotalAmount,
                OfficeSpaceId = request.OfficeSpaceId,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.BookingTransactions.Add(bookingTransaction);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Data Berhasi Ditambahkan");
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<BookingTransactionResponse>> GetBookingTransaction(int id)
        {
            var transaction = await _context.BookingTransactions
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.Photos)
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.Benefits)
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.City)
                .FirstOrDefaultAsync(bt => bt.Id == id);

            if (transaction == null)
            {
                return NotFound("Booking Transaction not found.");
            }

            var responseBody = new BookingTransactionResponse();
            responseBody.Status = 200;
            responseBody.Message = "Sukses";
            responseBody.Data = transaction;

            return responseBody;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookingTransactionAdmin(int id, [FromBody] BookingTransactionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookingTransaction = await _context.BookingTransactions.FindAsync(id);
            if (bookingTransaction == null)
            {
                return NotFound("Booking transaction not found.");
            }

            var officeSpace = await _context.OfficeSpaces.FindAsync(request.OfficeSpaceId);
            if (officeSpace == null)
            {
                return NotFound("Office space not found.");
            }

            bookingTransaction.Name = request.Name;
            bookingTransaction.PhoneNumber = request.PhoneNumber;
            bookingTransaction.BookingTrxId = BookingTransaction.GenerateUniqueTrxId(_context);
            bookingTransaction.Slug = request.Name.ToLower().Replace(" ", "-");
            bookingTransaction.IsPaid = request.IsPaid;
            bookingTransaction.Duration = officeSpace.Duration;
            bookingTransaction.StartedAt = request.StartedAt;
            bookingTransaction.EndedAt = request.StartedAt.AddDays(officeSpace.Duration);
            bookingTransaction.TotalAmount = request.TotalAmount;
            bookingTransaction.OfficeSpaceId = request.OfficeSpaceId;

            await _context.SaveChangesAsync();

            return Ok("Data Berhasil Diubah");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookingTransaction(int id)
        {
            var transaction = await _context.BookingTransactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound("Booking Transaction not found.");
            }

            _context.BookingTransactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok("Data Berhasil Dihapus");
        }

        
    }
}
