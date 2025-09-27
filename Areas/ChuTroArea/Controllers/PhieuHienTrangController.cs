using DACS_QuanLyPhongTro.Models;
using DACS_QuanLyPhongTro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
        [Area("ChuTroArea")]
        [Authorize(Roles = "ChuTro")]
        public class PhieuHienTrangController :Controller
        {
            public class PasswordVerificationModel
            {
                public string Password { get; set; }
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> VerifyPassword([FromBody] PasswordVerificationModel model)
            {
                var email = User.Identity.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });

                var signInManager = HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser>)) as Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser>;
                var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (result.Succeeded)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Mật khẩu không đúng." });
            }

            private readonly ApplicationDbContext _context;

            public PhieuHienTrangController(ApplicationDbContext context) 
            {
                _context = context;
            }

        // Hiển thị danh sách phòng có khách thuê để chọn
        public async Task<IActionResult> ChonPhong()
        {

            var email = User.Identity.Name;
            var chuTro = await _context.ChuTros
                .FirstOrDefaultAsync(c => c.Email == email);

            if (chuTro == null)
                return NotFound("Không tìm thấy chủ trọ.");

            var phongCoKhachThue = await _context.PhongTros
    .Where(p => p.TrangThai == "Đã Thuê" &&
                _context.ToaNhas
                    .Where(t => t.MaChuTro == chuTro.MaChuTro)
                    .Select(t => t.MaToaNha)
                    .Contains(p.MaToaNha))
    .Include(p => p.HopDongs)
        .ThenInclude(h => h.KhachThue)
    .ToListAsync();

            var phongLoc = phongCoKhachThue
                .Where(p => p.HopDongs.Any(h =>
                    !string.IsNullOrEmpty(h.TrangThai) &&
                    h.TrangThai.Trim().Equals("Đã Xác Nhận", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return View(phongLoc);

        }
        [HttpGet]
        public async Task<IActionResult> HienThiFormTaoPhieu(int maPhong)
        {
            var phong = await _context.PhongTros.FindAsync(maPhong);
            if (phong == null)
                return NotFound();

            var viewModel = new TaoPhieuHienTrangViewModel
            {
                MaPhong = maPhong,
                TenPhong = phong.SoPhong
            };

            return View(viewModel);
        }

        // Tạo phiếu hiện trạng nhận phòng và vật dụng
        [HttpPost]
        public async Task<IActionResult> TaoPhieu(TaoPhieuHienTrangViewModel model)
        {
            var hopDong = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.MaPhong == model.MaPhong && h.TrangThai == "Đã Xác Nhận");

            if (hopDong == null)
                return NotFound("Không tìm thấy hợp đồng.");

            var phieuTonTai = await _context.PhieuHienTrangNhanPhongs
                .AnyAsync(p => p.MaKhachThue == hopDong.MaKhachThue);

            if (phieuTonTai)
                return BadRequest("Phòng này đã có phiếu hiện trạng.");

            var phieu = new PhieuHienTrangNhanPhong
            {
                NgayNhanPhong = DateTime.Now,
                GhiChu = model.GhiChu,
                MaKhachThue = hopDong.MaKhachThue,
                TinhTrang = "Chưa xác nhận"
            };

            _context.PhieuHienTrangNhanPhongs.Add(phieu);
            await _context.SaveChangesAsync();

            foreach (var vatDung in model.VatDungs)
            {
                var item = new HienTrangVatDung
                {
                    MaPhieuHienTrang = phieu.MaPhieuHienTrang,
                    TenVatDung = vatDung.TenVatDung,
                    TinhTrang = vatDung.TinhTrang
                };
                _context.HienTrangVatDungs.Add(item);
            }

            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Tạo phiếu thành công.";
            return RedirectToAction("Index");
        }


        // Hiển thị danh sách phiếu hiện trạng và vật dụng
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

            var phieuHienTrangs = await _context.PhieuHienTrangNhanPhongs
                .Include(p => p.KhachThue)
                .Include(p => p.HienTrangVatDungs)
                .Where(p => maPhongChuTro.Contains(
                    _context.HopDongs
                        .Where(h => h.MaKhachThue == p.MaKhachThue)
                        .Select(h => h.MaPhong)
                        .FirstOrDefault() ?? -1
                ))
                .OrderByDescending(p => p.NgayNhanPhong)
                .ToListAsync();

            return View(phieuHienTrangs);
        }
    }
}