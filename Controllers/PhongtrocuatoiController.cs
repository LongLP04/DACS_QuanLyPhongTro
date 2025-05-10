using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    [Authorize(Roles = "KhachThue")]
    public class PhongtrocuatoiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhongtrocuatoiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Forbid("Bạn chưa đăng nhập.");

            var khachThue = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.ApplicationUserId == userId);

            if (khachThue == null)
                return NotFound("Không tìm thấy thông tin khách thuê.");

            var hopDongs = await _context.HopDongs
                .Where(h => h.MaKhachThue == khachThue.MaKhachThue && h.TrangThai == "Đã Xác Nhận")
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ToaNha)
                        .ThenInclude(t => t.ChuTro)
                .ToListAsync();

            var phongTros = hopDongs
                .Select(h => h.PhongTro)
                .Where(p => p != null)
                .ToList();

            return View(phongTros);
        }

    }
}
