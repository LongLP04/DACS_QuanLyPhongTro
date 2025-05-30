using DACS_QuanLyPhongTro.Models;
using DACS_QuanLyPhongTro.ViewModels;
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
        // Trả về ViewModel chứa danh sách dịch vụ + các lựa chọn mặc định
        public async Task<IActionResult> DangKy()
        {
            var dichVus = await _context.DichVus.ToListAsync();

            var viewModel = new PhieuDangKyDichVuViewModel
            {
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddMonths(1),
                DichVus = dichVus,
                SelectedDichVus = dichVus.Select(dv => new DichVuSelection
                {
                    MaDichVu = dv.MaDichVu,
                    IsSelected = false,
                    SoLuong = 1
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: /DichVu/DangKyNhieu
        // Xử lý đăng ký nhiều dịch vụ cùng lúc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyNhieu(PhieuDangKyDichVuViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid("Bạn chưa đăng nhập.");

            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.ApplicationUserId == userId);
            if (khachThue == null)
                return NotFound("Không tìm thấy thông tin khách thuê.");

            var selectedServices = model.SelectedDichVus
                .Where(s => s.IsSelected == true && s.SoLuong > 0)
                .ToList();

            if (!selectedServices.Any())
            {
                TempData["Error"] = "Bạn chưa chọn dịch vụ nào.";
                return RedirectToAction("DangKy");
            }

            // Tạo phiếu đăng ký chung cho tất cả dịch vụ lần này
            var phieu = new PhieuDangKyDichVu
            {
                MaKhachThue = khachThue.MaKhachThue,
                NgayBatDau = model.NgayBatDau,
                NgayKetThuc = model.NgayKetThuc,
                TrangThai = "Chờ xác nhận"
            };

            _context.PhieuDangKyDichVus.Add(phieu);
            await _context.SaveChangesAsync();

            // Thêm chi tiết cho từng dịch vụ đăng ký
            foreach (var item in selectedServices)
            {
                var dv = await _context.DichVus.FindAsync(item.MaDichVu);
                if (dv == null) continue;

                var chiTiet = new ChiTietPhieuDangKyDichVu
                {
                    MaDangKyDichVu = phieu.MaDangKyDichVu,
                    MaDichVu = dv.MaDichVu,
                    SoLuong = item.SoLuong,
                    TongTienDichVu = dv.DonGiaDichVu * item.SoLuong
                };

                _context.ChiTietPhieuDangKyDichVus.Add(chiTiet);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký dịch vụ thành công.";
            return RedirectToAction("DangKy");
        }

        // GET: /DichVu/DanhSachDangKy
        // Hiển thị danh sách các phiếu đăng ký dịch vụ của khách thuê
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
