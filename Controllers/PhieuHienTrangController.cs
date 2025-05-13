using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    [Authorize(Roles = "KhachThue")]
    public class PhieuHienTrangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuHienTrangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /PhieuHienTrang/XemPhieu
        public async Task<IActionResult> XemPhieu()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid("Bạn chưa đăng nhập.");

            var khachThue = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.ApplicationUserId == userId);
            if (khachThue == null)
                return NotFound("Không tìm thấy thông tin khách thuê.");

            var phieu = await _context.PhieuHienTrangNhanPhongs
                .Include(p => p.HienTrangVatDungs)
                .FirstOrDefaultAsync(p => p.MaKhachThue == khachThue.MaKhachThue);

            if (phieu == null)
                return View("KhongCoPhieu"); // View riêng nếu không có phiếu

            return View(phieu);
        }

        // POST: /PhieuHienTrang/XacNhan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(int maPhieu)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid();

            var khachThue = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.ApplicationUserId == userId);
            if (khachThue == null)
                return NotFound("Không tìm thấy khách thuê.");

            var phieu = await _context.PhieuHienTrangNhanPhongs
                .FirstOrDefaultAsync(p => p.MaPhieuHienTrang == maPhieu && p.MaKhachThue == khachThue.MaKhachThue);

            if (phieu == null)
                return NotFound("Không tìm thấy phiếu hiện trạng.");

            if (phieu.TinhTrang == "Đã xác nhận")
            {
                TempData["ThongBao"] = "Phiếu đã được xác nhận trước đó.";
                return RedirectToAction("XemPhieu");
            }

            phieu.TinhTrang = "Đã xác nhận";
            _context.Update(phieu);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Xác nhận phiếu hiện trạng thành công.";
            return RedirectToAction("XemPhieu");
        }
    }
}
