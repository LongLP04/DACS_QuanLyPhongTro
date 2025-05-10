using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Xem danh sách hóa đơn của các phòng trọ
        public async Task<IActionResult> Index()
        {
            var currentChuTroEmail = User.Identity.Name; // Lấy email của chủ trọ đang đăng nhập
            if (currentChuTroEmail == null)
            {
                return Forbid("Bạn chưa đăng nhập.");
            }

            // Tìm Chủ Trọ theo email
            var chuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (chuTro == null)
            {
                return NotFound("Chủ trọ không tồn tại.");
            }

            // Lấy danh sách phòng đã thuê
            var phongTrosDaThue = chuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();

            if (!phongTrosDaThue.Any())
            {
                return NotFound("Không có phòng nào đang được thuê.");
            }

            // Lấy danh sách hóa đơn liên quan đến các phòng
            var hoaDons = await _context.HoaDons
                .Where(h => phongTrosDaThue.Select(p => p.MaPhong).Contains(h.ChiSoDienNuoc.PhongTro.MaPhong))
                .Include(h => h.ChiSoDienNuoc)
                .ThenInclude(c => c.PhongTro)
                .Include(h => h.KhachThue)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();

            if (!hoaDons.Any())
            {
                return NotFound("Không có hóa đơn nào cho các phòng của bạn.");
            }

            // Kiểm tra hóa đơn mới của tháng hiện tại (May 2025)
            var currentMonth = new DateTime(2025, 5, 1); // Ngày hiện tại là 10/05/2025
            var hasCurrentMonthData = hoaDons.Any(h => h.NgayLap.Month == currentMonth.Month && h.NgayLap.Year == currentMonth.Year);

            if (hasCurrentMonthData)
            {
                TempData["ThongBao"] = "Có hóa đơn mới trong tháng này, kiểm tra ngay!";
            }

            return View(hoaDons);
        }
    }
}