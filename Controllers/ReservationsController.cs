using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Reservations — для гостя: его брони, для admin: все
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            IQueryable<Reservation> query = _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User);

            if (!User.IsInRole("Admin"))
                query = query.Where(r => r.UserId == user.Id);

            return View(await query.OrderByDescending(r => r.BookingDate).ToListAsync());
        }

        // GET: /Reservations/Create?roomId=1
        public async Task<IActionResult> Create(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || !room.IsAvailable) return NotFound();

            var model = new Reservation
            {
                RoomId = roomId,
                Room = room,
                CheckIn = DateTime.Today.AddDays(1),
                CheckOut = DateTime.Today.AddDays(2)
            };
            return View(model);
        }

        // POST: /Reservations/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Убираем ошибки навигационных свойств
            ModelState.Remove("User");
            ModelState.Remove("Room");
            ModelState.Remove("UserId");

            reservation.UserId = user.Id;

            if (reservation.CheckOut <= reservation.CheckIn)
                ModelState.AddModelError("CheckOut", "Дата выезда должна быть позже даты заезда.");

            // Проверка конфликта дат
            bool conflict = await _context.Reservations.AnyAsync(r =>
                r.RoomId == reservation.RoomId &&
                r.Id != reservation.Id &&
                r.CheckIn < reservation.CheckOut &&
                r.CheckOut > reservation.CheckIn);

            if (conflict)
                ModelState.AddModelError("", "Этот номер уже занят на выбранные даты.");

            if (ModelState.IsValid)
            {
                var room = await _context.Rooms.FindAsync(reservation.RoomId);
                if (room == null) return NotFound();

                reservation.TotalPrice = room.PricePerNight * reservation.Nights;
                reservation.BookingDate = DateTime.Now;

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Номер успешно забронирован!";
                return RedirectToAction(nameof(Index));
            }

            reservation.Room = await _context.Rooms.FindAsync(reservation.RoomId);
            return View(reservation);
        }

        // GET: /Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null) return NotFound();
            if (!User.IsInRole("Admin") && reservation.UserId != user!.Id) return Forbid();

            return View(reservation);
        }

        // POST: /Reservations/Cancel/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null) return NotFound();
            if (!User.IsInRole("Admin") && reservation.UserId != user!.Id) return Forbid();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Бронирование отменено.";
            return RedirectToAction(nameof(Index));
        }
    }
}
