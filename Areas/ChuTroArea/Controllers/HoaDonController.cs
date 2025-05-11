using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Xem danh sách hóa đơn của các phòng trọ
        public async Task<IActionResult> Index()
        {
            var currentChuTroEmail = User.Identity.Name;
            if (currentChuTroEmail == null)
            {
                return Forbid("Bạn chưa đăng nhập.");
            }

            var chuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (chuTro == null)
            {
                return NotFound("Chủ trọ không tồn tại.");
            }

            var phongTrosDaThue = chuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();

            if (!phongTrosDaThue.Any())
            {
                return NotFound("Không có phòng nào đang được thuê.");
            }

            var hoaDons = await _context.HoaDons
                .Where(h => phongTrosDaThue.Select(p => p.MaPhong).Contains(h.ChiSoDienNuoc.PhongTro.MaPhong))
                .Include(h => h.ChiSoDienNuoc)
                .ThenInclude(c => c.PhongTro)
                .Include(h => h.KhachThue)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();

            if (!hoaDons.Any())
            {
                ViewBag.ThongBao = "Hiện chưa có hóa đơn nào được tạo cho các phòng.";
            }

            return View(hoaDons);
        }
        // Trong HoaDonController
        public async Task<IActionResult> Details(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.PhieuThanhToans)
                .ThenInclude(p => p.PhuongThucThanhToan)
                .Include(h => h.KhachThue)
                .Include(h => h.ChiSoDienNuoc)
                .ThenInclude(c => c.PhongTro)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // Action để tạo hóa đơn
        public IActionResult Create()
        {
            // Lấy danh sách phòng trọ để người dùng chọn
            var phongTros = _context.PhongTros.ToList();

            // Truyền danh sách phòng vào View
            ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");

            return View();
        }

        // Action xử lý khi người dùng submit form
        [HttpPost]
        public IActionResult Create(HoaDon hoaDon)
        {
            // Tìm thông tin chỉ số điện nước
            var chiSoDienNuoc = _context.ChiSoDienNuocs
                .Where(c => c.MaPhong == hoaDon.MaPhong)
                .OrderByDescending(c => c.NgayGhi)
                .FirstOrDefault();

            if (chiSoDienNuoc == null)
            {
                ModelState.AddModelError("", "Không tìm thấy chỉ số điện nước cho phòng này.");
                ViewBag.PhongTros = new SelectList(_context.PhongTros, "MaPhong", "SoPhong");
                return View(hoaDon);
            }

            // Gán MaChiSo
            hoaDon.MaChiSo = chiSoDienNuoc.MaChiSo;

            // Tìm thông tin phòng và khách thuê
            var phong = _context.PhongTros
                .Include(p => p.KhachThue) // Giả sử PhongTro có navigation property đến KhachThue
                .FirstOrDefault(p => p.MaPhong == hoaDon.MaPhong);

            if (phong == null) { 
                    ModelState.AddModelError("", "Phòng không tồn tại.");
                    ViewBag.PhongTros = new SelectList(_context.PhongTros, "MaPhong", "SoPhong");
                    return View(hoaDon);
        }

        // Kiểm tra xem phòng có khách thuê không
        var khachThue = _context.KhachThues
            .FirstOrDefault(k => k.MaKhachThue == phong.MaKhachThue); // Giả sử PhongTro có MaKhachThue

    if (khachThue == null)
    {
        ModelState.AddModelError("", "Phòng này chưa có khách thuê.");
        ViewBag.PhongTros = new SelectList(_context.PhongTros, "MaPhong", "SoPhong");
        return View(hoaDon);
    }

    // Gán MaKhachThue
    hoaDon.MaKhachThue = khachThue.MaKhachThue;

            // ✅ Tính tiền dịch vụ chính xác
            var phieuDichVus = _context.PhieuDangKyDichVus
                .Where(p => p.MaKhachThue == khachThue.MaKhachThue && p.TrangThai == "Đã xác nhận")
                .Include(p => p.ChiTietPhieuDangKyDichVus)
                .ToList();

            decimal tongTienDichVu = phieuDichVus
                .SelectMany(p => p.ChiTietPhieuDangKyDichVus)
                .Sum(ct => ct.TongTienDichVu);

            hoaDon.TienDichVu = tongTienDichVu;
            // Tính toán hóa đơn
            hoaDon.TienDien = chiSoDienNuoc.SoDienTieuThu* chiSoDienNuoc.DonGiaDien;
    hoaDon.TienNuoc = chiSoDienNuoc.SoNuocTieuThu* chiSoDienNuoc.DonGiaNuoc;
    hoaDon.TienPhong = phong.GiaThue;
    hoaDon.TongTien = hoaDon.TienDien + hoaDon.TienNuoc + hoaDon.TienPhong + hoaDon.TienDichVu;
    hoaDon.NgayLap = DateTime.Now;
            hoaDon.TrangThai = "Chưa thanh toán";

            // Lưu hóa đơn
            _context.HoaDons.Add(hoaDon);
    _context.SaveChanges();

    return RedirectToAction("Index");
}

        public async Task<IActionResult> XacNhanThanhToan(int id)
        {
            // Lấy thông tin hóa đơn
            var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null || hoaDon.TrangThai != "Chờ xác nhận")
            {
                return NotFound();
            }

            // Cập nhật trạng thái hóa đơn thành "Đã thanh toán"
            hoaDon.TrangThai = "Đã thanh toán";
            _context.SaveChanges();

            return RedirectToAction("Index"); // Quay lại danh sách hóa đơn
        }

    }
}
