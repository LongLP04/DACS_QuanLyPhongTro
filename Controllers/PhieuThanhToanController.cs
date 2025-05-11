using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DACS_QuanLyPhongTro.Areas.KhachThueArea.Controllers
{
    [Authorize(Roles = "KhachThue")]

    public class PhieuThanhToanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuThanhToanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách hóa đơn chưa thanh toán
        public async Task<IActionResult> Index()
        {
            var email = User.Identity.Name;
            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.Email == email);

            if (khachThue == null)
                return Forbid();

            var hoaDons = await _context.HoaDons
                .Include(h => h.PhongTro)
                .Where(h => h.MaKhachThue == khachThue.MaKhachThue && h.TrangThai == "Chưa thanh toán")
                .ToListAsync();

            return View(hoaDons);
        }

        // Form thanh toán
        public async Task<IActionResult> ThanhToan(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null || hoaDon.TrangThai == "Đã thanh toán")
                return NotFound();

            var phuongThucs = _context.PhuongThucThanhToans.ToList();
            ViewBag.PhuongThuc = phuongThucs;
            return View(hoaDon);
        }

        // Xử lý thanh toán (POST)
        [HttpPost]
        public IActionResult XacNhanThanhToan(int id, int maPhuongThuc)
        {
            // Lấy thông tin hóa đơn
            var hoaDon = _context.HoaDons.FirstOrDefault(h => h.MaHoaDon == id);
            if (hoaDon == null)
            {
                // Xử lý nếu không tìm thấy hóa đơn
                return NotFound();
            }

            // Lấy phương thức thanh toán
            var phuongThucThanhToan = _context.PhuongThucThanhToans.FirstOrDefault(pt => pt.MaPhuongThuc == maPhuongThuc);

            if (phuongThucThanhToan == null)
            {
                // Xử lý nếu không tìm thấy phương thức thanh toán
                return NotFound();
            }

            // Tạo phiếu thanh toán
            var phieuThanhToan = new PhieuThanhToan
            {
                NgayThanhToan = DateTime.Now,
                SoTienThanhToan = hoaDon.TongTien,
                MaHoaDon = hoaDon.MaHoaDon,
                MaPhuongThuc = phuongThucThanhToan.MaPhuongThuc,
                PhuongThucThanhToan = phuongThucThanhToan
            };

            // Lưu phiếu thanh toán
            _context.PhieuThanhToans.Add(phieuThanhToan);
            _context.SaveChanges();

            // Cập nhật trạng thái hóa đơn thành "Đã thanh toán"
            hoaDon.TrangThai = "Chờ xác nhận";
            _context.SaveChanges();

            // Quay lại trang danh sách hóa đơn hoặc thông báo thành công
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> LichSuThanhToan()
        {
            var email = User.Identity.Name;
            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.Email == email);

            if (khachThue == null)
                return Forbid();

            var lichSu = await _context.PhieuThanhToans
                .Include(p => p.HoaDon).ThenInclude(h => h.PhongTro)
                .Include(p => p.PhuongThucThanhToan)
                .Where(p => p.HoaDon.MaKhachThue == khachThue.MaKhachThue)
                .OrderByDescending(p => p.NgayThanhToan)
                .ToListAsync();

            return View(lichSu);
        }



    }
}
