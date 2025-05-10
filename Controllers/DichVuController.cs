using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    [Authorize(Roles = "KhachThue")]
    public class DichVuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /DichVu/DangKy
        public async Task<IActionResult> DangKy()
        {
            var dichVus = await _context.DichVus.ToListAsync();
            return View(dichVus);
        }

        // POST: /DichVu/DangKyMot
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyMot(int maDichVu, int soLuong, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid("Bạn chưa đăng nhập.");

            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.ApplicationUserId == userId);
            if (khachThue == null)
                return NotFound("Không tìm thấy thông tin khách thuê.");

            var dichVu = await _context.DichVus.FindAsync(maDichVu);
            if (dichVu == null || soLuong <= 0)
                return BadRequest("Dịch vụ không hợp lệ.");

            var phieu = new PhieuDangKyDichVu
            {
                MaKhachThue = khachThue.MaKhachThue,
                NgayBatDau = ngayBatDau,
                NgayKetThuc = ngayBatDau.AddMonths(1),
                TrangThai = "Chờ xác nhận"
            };

            _context.PhieuDangKyDichVus.Add(phieu);
            await _context.SaveChangesAsync();

            var chiTiet = new ChiTietPhieuDangKyDichVu
            {
                MaDangKyDichVu = phieu.MaDangKyDichVu,
                MaDichVu = dichVu.MaDichVu,
                SoLuong = soLuong,
                TongTienDichVu = dichVu.DonGiaDichVu * soLuong
            };

            _context.ChiTietPhieuDangKyDichVus.Add(chiTiet);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký dịch vụ thành công.";
            return RedirectToAction("DangKy");
        }
        // GET: /DichVu/DanhSachDangKy
        public async Task<IActionResult> DanhSachDangKy()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid("Bạn chưa đăng nhập.");

            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.ApplicationUserId == userId);
            if (khachThue == null)
                return NotFound("Không tìm thấy thông tin khách thuê.");

            var danhSachPhieu = await _context.PhieuDangKyDichVus
                .Where(p => p.MaKhachThue == khachThue.MaKhachThue)
                .Include(p => p.ChiTietPhieuDangKyDichVus)
                    .ThenInclude(ct => ct.DichVu)
                .OrderByDescending(p => p.NgayBatDau)
                .ToListAsync();

            return View(danhSachPhieu);
        }

    }
}
