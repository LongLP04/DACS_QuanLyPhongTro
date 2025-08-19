using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    public class HopDongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HopDongController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Xem chi tiết hợp đồng
        [HttpGet]
        [Authorize(Roles = "KhachThue")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var hopDong = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ToaNha)
                        .ThenInclude(t => t.ChuTro)  // Phải include chu tro
                .FirstOrDefaultAsync(h => h.MaHopDong == id);


            if (hopDong == null)
                return NotFound();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.ApplicationUserId == currentUserId);

            if (khachThue == null || hopDong.MaKhachThue != khachThue.MaKhachThue)
                return Forbid();

            return View(hopDong);
        }

        // GET: Xác nhận hợp đồng
        [HttpGet]
        [Authorize(Roles = "KhachThue")]
        public async Task<IActionResult> XacNhan()
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

            // Lọc hợp đồng của khách thuê với trạng thái "Chờ Xác Nhận"
            var hopDongs = await _context.HopDongs
                .Where(h => h.MaKhachThue == khachThue.MaKhachThue && h.TrangThai == "Chờ Xác Nhận")
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .ToListAsync();



            // Lưu số lượng hợp đồng chờ xác nhận vào session
            HttpContext.Session.SetInt32("HopDongCount", hopDongs.Count);

            return View(hopDongs); // Trả về danh sách hợp đồng chờ xác nhận
        }

        // POST: Xác nhận hợp đồng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "KhachThue")]
        public async Task<IActionResult> XacNhan(int maHopDong, bool dongY)
        {
            var hopDong = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.MaHopDong == maHopDong);

            if (hopDong == null) return NotFound("Không tìm thấy hợp đồng.");

            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (currentUserEmail != hopDong.KhachThue?.Email)
            {
                return Forbid("Bạn không có quyền xác nhận hợp đồng này.");
            }

            if (dongY)
            {
                hopDong.TrangThai = "Đã Xác Nhận";
                var phongTro = await _context.PhongTros
                    .FirstOrDefaultAsync(p => p.MaPhong == hopDong.MaPhong);
                if (phongTro != null)
                {
                    phongTro.TrangThai = "Đã Thuê";
                    phongTro.MaKhachThue = hopDong.MaKhachThue;
                    _context.Update(phongTro);
                }

                TempData["ThongBao"] = "Bạn đã xác nhận hợp đồng thành công. Trạng thái phòng đã được cập nhật.";
                HttpContext.Session.Remove("MaHopDong");
            }
            else
            {
                hopDong.TrangThai = "Từ chối xác nhận";
                var phongTro = await _context.PhongTros
                    .FirstOrDefaultAsync(p => p.MaPhong == hopDong.MaPhong);
                if (phongTro != null)
                {
                    phongTro.TrangThai = "Trống";
                    _context.Update(phongTro);
                }
                TempData["ThongBao"] = "Bạn đã từ chối xác nhận hợp đồng.";
                HttpContext.Session.Remove("MaHopDong");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        // POST: Từ chối hợp đồng

    }
}