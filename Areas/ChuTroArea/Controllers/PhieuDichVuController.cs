using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class PhieuDichVuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuDichVuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang chính: Hiển thị dịch vụ đã xác nhận + cảnh báo nếu có phiếu chờ xác nhận
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

            // Lấy các phiếu đã xác nhận
            var phieuDaXacNhan = await _context.PhieuDangKyDichVus
                .Include(p => p.KhachThue)
                .Include(p => p.ChiTietPhieuDangKyDichVus)
                    .ThenInclude(ct => ct.DichVu)
                .Where(p => p.TrangThai == "Đã xác nhận" &&
                            maPhongChuTro.Contains(
                                _context.HopDongs
                                    .Where(h => h.MaKhachThue == p.MaKhachThue)
                                    .Select(h => h.MaPhong)
                                    .FirstOrDefault() ?? -1
                            ))
                .ToListAsync();

            // Kiểm tra có phiếu đang chờ xác nhận không
            bool coPhieuCho = await _context.PhieuDangKyDichVus
                .AnyAsync(p => p.TrangThai == "Chờ xác nhận" &&
                               maPhongChuTro.Contains(
                                   _context.HopDongs
                                       .Where(h => h.MaKhachThue == p.MaKhachThue)
                                       .Select(h => h.MaPhong)
                                       .FirstOrDefault() ?? -1
                               ));

            ViewBag.CoPhieuCho = coPhieuCho;

            return View(phieuDaXacNhan);
        }

        // Hiển thị danh sách phiếu đang chờ xác nhận
        public async Task<IActionResult> DanhSachChoXacNhan()
        {
            var email = User.Identity.Name;
            var chuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                    .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == email);

            var maPhongChuTro = chuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Select(p => p.MaPhong)
                .ToList();

            var phieuDangKy = await _context.PhieuDangKyDichVus
                .Include(p => p.KhachThue)
                .Include(p => p.ChiTietPhieuDangKyDichVus)
                    .ThenInclude(ct => ct.DichVu)
                .Where(p => p.TrangThai == "Chờ xác nhận" &&
                            maPhongChuTro.Contains(
                                _context.HopDongs
                                    .Where(h => h.MaKhachThue == p.MaKhachThue)
                                    .Select(h => h.MaPhong)
                                    .FirstOrDefault() ?? -1))
                .ToListAsync();

            return View(phieuDangKy);
        }

        // POST: Xác nhận phiếu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(int id)
        {
            var phieu = await _context.PhieuDangKyDichVus
                .FirstOrDefaultAsync(p => p.MaDangKyDichVu == id);

            if (phieu == null)
                return NotFound();

            phieu.TrangThai = "Đã xác nhận";
            _context.Update(phieu);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Phiếu đã được xác nhận.";
            return RedirectToAction("Index");
        }
        // Hiển thị danh sách dịch vụ
        public async Task<IActionResult> DanhSachDichVu()
        {
            var dichVus = await _context.DichVus.ToListAsync();
            return View(dichVus);
        }

        // GET: Tạo dịch vụ
        public IActionResult TaoDichVu()
        {
            return View();
        }

        // POST: Tạo dịch vụ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoDichVu(DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dichVu);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "Thêm dịch vụ thành công.";
                return RedirectToAction("DanhSachDichVu");
            }
            return View(dichVu);
        }

        // GET: Sửa dịch vụ
        public async Task<IActionResult> SuaDichVu(int id)
        {
            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu == null)
                return NotFound();
            return View(dichVu);
        }

        // POST: Sửa dịch vụ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaDichVu(int id, DichVu dichVu)
        {
            if (id != dichVu.MaDichVu)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(dichVu);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "Cập nhật dịch vụ thành công.";
                return RedirectToAction("DanhSachDichVu");
            }
            return View(dichVu);
        }

        // POST: Xóa dịch vụ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaDichVu(int id)
        {
            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu != null)
            {
                _context.DichVus.Remove(dichVu);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "Đã xóa dịch vụ.";
            }
            return RedirectToAction("DanhSachDichVu");
        }
        // GET: Chi tiết phiếu đăng ký dịch vụ
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var phieu = await _context.PhieuDangKyDichVus
                .Include(p => p.KhachThue)
                .Include(p => p.ChiTietPhieuDangKyDichVus)
                    .ThenInclude(ct => ct.DichVu)
                .FirstOrDefaultAsync(p => p.MaDangKyDichVu == id);

            if (phieu == null) return NotFound();

            return View(phieu);
        }

        // POST: Xóa phiếu đăng ký dịch vụ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaPhieu(int id)
        {
            var phieu = await _context.PhieuDangKyDichVus
                .Include(p => p.ChiTietPhieuDangKyDichVus)
                .FirstOrDefaultAsync(p => p.MaDangKyDichVu == id);

            if (phieu == null) return NotFound();

            // Xóa chi tiết phiếu trước nếu có (nếu có liên kết FK)
            if (phieu.ChiTietPhieuDangKyDichVus != null)
            {
                _context.ChiTietPhieuDangKyDichVus.RemoveRange(phieu.ChiTietPhieuDangKyDichVus);
            }

            _context.PhieuDangKyDichVus.Remove(phieu);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Phiếu đăng ký dịch vụ đã được xóa.";
            return RedirectToAction(nameof(Index));
        }

    }
}
