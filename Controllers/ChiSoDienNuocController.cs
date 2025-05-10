using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    [Authorize(Roles = "KhachThue")]

    public class ChiSoDienNuocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChiSoDienNuocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Xem chi tiết chỉ số điện nước của phòng đang thuê
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Lấy ApplicationUserId từ Claim
            if (currentUserId == null)
            {
                return Forbid("Bạn chưa đăng nhập.");
            }

            // Tìm Khách Thuê theo ApplicationUserId
            var khachThue = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.ApplicationUserId == currentUserId);

            if (khachThue == null)
            {
                return NotFound("Khách thuê không tồn tại.");
            }

            // Tìm phòng mà khách thuê đang thuê (Trạng thái "Đã Thuê" và MaKhachThue khớp)
            var phongTro = await _context.PhongTros
                .FirstOrDefaultAsync(p => p.MaKhachThue == khachThue.MaKhachThue && p.TrangThai == "Đã Thuê");

            if (phongTro == null)
            {
                return NotFound("Bạn chưa thuê phòng nào.");
            }

            // Lấy danh sách chỉ số điện nước của phòng
            var chiSoDienNuocs = await _context.ChiSoDienNuocs
                .Where(c => c.MaPhong == phongTro.MaPhong)
                .Include(c => c.PhongTro)
                .OrderByDescending(c => c.NgayGhi)
                .ToListAsync();

            if (!chiSoDienNuocs.Any())
            {
                return NotFound("Không có chỉ số điện nước nào cho phòng của bạn.");
            }

            // Kiểm tra chỉ số điện nước của tháng hiện tại (May 2025)
            var currentMonth = new DateTime(2025, 5, 1); // Ngày hiện tại là 10/05/2025
            var hasCurrentMonthData = chiSoDienNuocs.Any(c => c.NgayGhi.Month == currentMonth.Month && c.NgayGhi.Year == currentMonth.Year);

            if (hasCurrentMonthData)
            {
                TempData["ThongBao"] = "Chỉ số điện nước tháng này đã có, kiểm tra ngay!";
            }

            return View(chiSoDienNuocs);
        }
    }
}