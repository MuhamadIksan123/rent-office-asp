using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentOffice.Data;
using RentOffice.Models;
using RentOffice.Payload.Request;
using RentOffice.Payload.Response;
// using Twilio;
// using Twilio.Rest.Api.V2010.Account;
// using Twilio.Types;


namespace RentOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingTransactionUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingTransactionController> _logger;
        private readonly IConfiguration _configuration;

        public BookingTransactionUserController(ApplicationDbContext context, ILogger<BookingTransactionController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("store")]
        public async Task<ActionResult<BookingTransactionResponse>> Store([FromBody] UserBookingTransactionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var officeSpace = await _context.OfficeSpaces.FindAsync(request.OfficeSpaceId);
            if (officeSpace == null)
            {
                return NotFound("Office space not found");
            }

            var bookingTransaction = new BookingTransaction
            {
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                OfficeSpaceId = request.OfficeSpaceId,
                Slug = request.Name.ToLower().Replace(" ", "-"),
                IsPaid = false,
                BookingTrxId = BookingTransaction.GenerateUniqueTrxId(_context),
                StartedAt = request.StartedAt,
                EndedAt = request.StartedAt.AddDays(officeSpace.Duration),
                Duration = officeSpace.Duration
            };

            _context.BookingTransactions.Add(bookingTransaction);
            await _context.SaveChangesAsync();

            var responseBody = new BookingTransactionResponse
            {
                Status = 201,
                Message = "Success",
                Data = await _context.BookingTransactions
                    .Include(bt => bt.OfficeSpace)
                        .ThenInclude(os => os.City)
                    .Include(bt => bt.OfficeSpace)
                        .ThenInclude(os => os.Photos)
                    .Include(bt => bt.OfficeSpace)
                        .ThenInclude(os => os.Benefits)
                    .FirstOrDefaultAsync(bt => bt.Id == bookingTransaction.Id)
            };

            // Kirim notifikasi via Twilio WhatsApp
            // string accountSid = _configuration["Twilio:AccountSid"];
            // string authToken = _configuration["Twilio:AuthToken"];

            // if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
            // {
            //     return StatusCode(500, "Twilio credentials are missing!");
            // }

            // Inisialisasi Twilio
            // TwilioClient.Init(accountSid, authToken);

            // Membuat body pesan untuk WhatsApp
            // var messageBody = $"Hi ({bookingTransaction.Name}), Terima kasih telah booking kantor di FirstOffice.\n\n" +
            //                   $"Pesanan kantor ({officeSpace.Name}) Anda sedang kami proses dengan Booking TRX ID: {bookingTransaction.BookingTrxId}.\n\n" +
            //                   "Kami akan menginformasikan kembali status pemesanan Anda secepat mungkin.";

            // try
            // {
            //     // Kirim pesan WhatsApp
            //     var message = MessageResource.Create(
            //         from: new PhoneNumber("whatsapp:+14155238886"), // Nomor pengirim WhatsApp Twilio
            //         to: new PhoneNumber($"whatsapp:+{bookingTransaction.PhoneNumber}"), // Nomor penerima
            //         body: messageBody
            //     );
            // }
            // catch (Exception ex)
            // {
            //     // Menangani error jika pengiriman pesan gagal
            //     return StatusCode(500, $"Failed to send WhatsApp message: {ex.Message}");
            // }

            // Kembalikan response dengan informasi transaksi booking
            return responseBody;
        }



        [HttpGet("booking-details")]
        public async Task<ActionResult<ViewBookingResponse>> BookingDetails([FromQuery] string phoneNumber, [FromQuery] string bookingTrxId)
        {
            var bookingData = await _context.BookingTransactions
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.Photos)
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.Benefits)
                .Include(bt => bt.OfficeSpace)
                    .ThenInclude(os => os.City)
                .FirstOrDefaultAsync(b => b.PhoneNumber == phoneNumber && b.BookingTrxId == bookingTrxId);

            if (bookingData == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            // Buat instance ViewBookingResponse setelah mendapatkan data
            var responseBody = new ViewBookingResponse
            {
                Status = 200,
                Message = "Success",
                Data = bookingData
            };

            return responseBody;
        }
    }
}
