using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    [Authorize(Roles = "KhachThue")]
    public class SuCoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuCoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachThue = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.ApplicationUserId == userId);

            if (khachThue == null)
            {
                return NotFound("Khách thuê không tồn tại.");
            }

            var phieuSuCos = await _context.PhieuGhiNhanSuCos
                .Where(p => p.MaKhachThue == khachThue.MaKhachThue)
                .OrderByDescending(p => p.NgayGhiNhan)
                .ToListAsync();

            return View(phieuSuCos); // Trả về view ngay cả khi danh sách rỗng
        }

        public IActionResult TaoPhieu()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoPhieu(PhieuGhiNhanSuCo phieu)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.ApplicationUserId == userId);
            if (khachThue == null)
            {
                return Unauthorized();
            }

            phieu.MaKhachThue = khachThue.MaKhachThue;
            phieu.NgayGhiNhan = DateTime.Now;
            phieu.TinhTrang = "Chưa xử lý";

            _context.Add(phieu);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Gửi phiếu ghi nhận sự cố thành công.";
            return RedirectToAction("Index");
        }
    }
}