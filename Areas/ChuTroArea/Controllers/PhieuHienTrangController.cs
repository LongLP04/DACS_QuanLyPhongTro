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
    public class PhieuHienTrangController : Controller
    {
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

            return View(phongCoKhachThue);
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
                .FirstOrDefaultAsync(h => h.MaPhong == model.MaPhong && h.TrangThai == "Đã xác nhận");

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