using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Rooms
        public async Task<IActionResult> Index(string? type, decimal? maxPrice, int? capacity)
        {
            var query = _context.Rooms.AsQueryable();

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<RoomType>(type, out var roomType))
                query = query.Where(r => r.Type == roomType);

            if (maxPrice.HasValue)
                query = query.Where(r => r.PricePerNight <= maxPrice.Value);

            if (capacity.HasValue)
                query = query.Where(r => r.Capacity >= capacity.Value);

            ViewBag.SelectedType = type;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Capacity = capacity;

            return View(await query.ToListAsync());
        }

        // GET: /Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null) return NotFound();

            return View(room);
        }

        // ========== ADMIN CRUD ==========

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Номер успешно добавлен.";
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Номер обновлён.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Rooms.Any(r => r.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Номер удалён.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
