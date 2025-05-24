using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class PhieuSuCoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuSuCoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang chính: Hiển thị danh sách phiếu sự cố + cảnh báo nếu có phiếu chưa xử lý
        public async Task<IActionResult> Index()
        {
            string hoTen = "Chủ trọ";

            string userEmail = null; // Đổi tên biến email thành userEmail

            if (User.Identity.IsAuthenticated)
            {
                userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var chuTroInfo = await _context.ChuTros.FirstOrDefaultAsync(c => c.Email == userEmail);
                    if (chuTroInfo != null)
                    {
                        hoTen = chuTroInfo.HoTen;
                    }
                }
            }

            ViewData["ChuTroHoTen"] = hoTen;

            var email = User.Identity.Name;
            var chuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                    .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == email);

            if (chuTro == null)
                return NotFound("Không tìm thấy chủ trọ.");

            var maPhongChuTro = chuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Select(p => p.MaPhong)
                .ToList();

            // Lấy các phiếu sự cố từ khách thuê của phòng trọ thuộc chủ trọ
            var phieuSuCos = await _context.PhieuGhiNhanSuCos
                .Include(p => p.KhachThue)
                .Where(p => maPhongChuTro.Contains(
                    _context.HopDongs
                        .Where(h => h.MaKhachThue == p.MaKhachThue)
                        .Select(h => h.MaPhong)
                        .FirstOrDefault() ?? -1
                ))
                .OrderByDescending(p => p.NgayGhiNhan)
                .ToListAsync();

            // Kiểm tra có phiếu chưa xử lý không
            bool coPhieuChuaXuLy = phieuSuCos.Any(p => p.TinhTrang == "Chưa xử lý");

            ViewBag.CoPhieuChuaXuLy = coPhieuChuaXuLy;

            return View(phieuSuCos);
        }

        // POST: Xác nhận và cập nhật trạng thái phiếu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(int id, string tinhTrang)
        {
            if (string.IsNullOrEmpty(tinhTrang) || (tinhTrang != "Đã tiếp nhận" && tinhTrang != "Đã xử lý"))
                return BadRequest("Trạng thái không hợp lệ.");

            var phieu = await _context.PhieuGhiNhanSuCos
                .FirstOrDefaultAsync(p => p.MaPhieuSuCo == id);

            if (phieu == null)
                return NotFound();

            phieu.TinhTrang = tinhTrang;
            _context.Update(phieu);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"Phiếu đã được cập nhật thành trạng thái {tinhTrang}.";
            return RedirectToAction("Index");
        }
    }
}