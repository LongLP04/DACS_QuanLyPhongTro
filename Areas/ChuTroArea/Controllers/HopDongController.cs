using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class HopDongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HopDongController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Danh sách hợp đồng
        public async Task<IActionResult> Index()
        {
            string hoTen = "Chủ trọ";

            if (User.Identity.IsAuthenticated)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(email))
                {
                    var chuTro = await _context.ChuTros.FirstOrDefaultAsync(c => c.Email == email);
                    if (chuTro != null)
                    {
                        hoTen = chuTro.HoTen;
                    }
                }
            }

            ViewData["ChuTroHoTen"] = hoTen; // Truyền xuống layout
            var hopDongs = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .ToListAsync();

            return View(hopDongs);
        }

        // GET: Tạo hợp đồng
        public async Task<IActionResult> Create()
        {
            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null)
            {
                return NotFound("Không tìm thấy thông tin chủ trọ.");
            }

            // Lấy phòng trống của chủ trọ
            var phongTros = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Trống")
                .ToList();

            ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");
            return View();
        }

        // POST: Tạo hợp đồng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HopDong hopDong, string emailKhachThue)
        {
            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            // Kiểm tra email khách thuê
            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.Email == emailKhachThue);
            if (khachThue == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy khách thuê với email này.");
                var phongTros = currentChuTro.ToaNhas
                    .SelectMany(t => t.PhongTros)
                    .Where(p => p.TrangThai == "Trống")
                    .ToList();
                ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");
                return View(hopDong);
            }

            // Kiểm tra phòng thuộc chủ trọ và còn trống
            var phongTro = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .FirstOrDefault(p => p.MaPhong == hopDong.MaPhong);

            if (phongTro == null)
            {
                ModelState.AddModelError("MaPhong", "Phòng này không thuộc về bạn hoặc không tồn tại.");
                var phongTros = currentChuTro.ToaNhas
                    .SelectMany(t => t.PhongTros)
                    .Where(p => p.TrangThai == "Trống")
                    .ToList();
                ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");
                return View(hopDong);
            }

            if (phongTro.TrangThai != "Trống")
            {
                ModelState.AddModelError("MaPhong", "Phòng này hiện không trống.");
                var phongTros = currentChuTro.ToaNhas
                    .SelectMany(t => t.PhongTros)
                    .Where(p => p.TrangThai == "Trống")
                    .ToList();
                ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");
                return View(hopDong);
            }

            // Kiểm tra ngày bắt đầu hợp đồng mới phải sau ngày kết thúc hợp đồng trước
            var latestContract = await _context.HopDongs
                .Where(h => h.MaPhong == hopDong.MaPhong)
                .OrderByDescending(h => h.NgayKetThuc)
                .FirstOrDefaultAsync();

            if (latestContract != null && hopDong.NgayBatDau <= latestContract.NgayKetThuc)
            {
                ModelState.AddModelError("NgayBatDau", $"Ngày bắt đầu phải sau ngày kết thúc hợp đồng trước ({latestContract.NgayKetThuc:dd/MM/yyyy}).");
                var phongTros = currentChuTro.ToaNhas
                    .SelectMany(t => t.PhongTros)
                    .Where(p => p.TrangThai == "Trống")
                    .ToList();
                ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");
                return View(hopDong);
            }

            hopDong.MaKhachThue = khachThue.MaKhachThue;
            hopDong.NgayLap = DateTime.Now;

            _context.HopDongs.Add(hopDong);
            await _context.SaveChangesAsync();

            // Cập nhật trạng thái phòng thành "Đã Thuê"
            phongTro.TrangThai = "Đã Thuê";

            _context.PhongTros.Update(phongTro);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Hợp đồng đã được tạo. Đang chờ khách thuê xác nhận.";

            return RedirectToAction("Index");
        }

        // GET: Chi tiết hợp đồng
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var hopDong = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.MaHopDong == id);

            if (hopDong == null)
                return NotFound();

            return View(hopDong);
        }

        // GET: Xác nhận xóa hợp đồng
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var hopDong = await _context.HopDongs
                .Include(h => h.KhachThue)
                    .ThenInclude(k => k.PhieuGhiNhanSuCos)      // chú ý 'PhieuGhiNhanSuCos' (có s)
                .Include(h => h.KhachThue)
                    .ThenInclude(k => k.PhieuHienTrangNhanPhongs)  // chú ý 'PhieuHienTrangNhanPhongs' (có s)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.MaHopDong == id);

            if (hopDong == null)
                return NotFound();

            return View(hopDong);
        }

        // POST: Thực hiện xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong != null)
            {
                // Xóa hóa đơn và chỉ số điện nước liên quan
                var chiSoDienNuocList = await _context.ChiSoDienNuocs
                    .Where(c => c.MaPhong == hopDong.MaPhong)
                    .ToListAsync();

                if (chiSoDienNuocList.Any())
                {
                    var maChiSoList = chiSoDienNuocList.Select(c => c.MaChiSo).ToList();

                    var hoaDonList = await _context.HoaDons
                        .Where(hd => maChiSoList.Contains(hd.MaChiSo))
                        .ToListAsync();

                    if (hoaDonList.Any())
                    {
                        _context.HoaDons.RemoveRange(hoaDonList);
                    }

                    _context.ChiSoDienNuocs.RemoveRange(chiSoDienNuocList);
                }

                // Xóa phiếu ghi nhận sự cố của khách thuê
                var phieuSuCoList = await _context.PhieuGhiNhanSuCos
                    .Where(p => p.MaKhachThue == hopDong.MaKhachThue)
                    .ToListAsync();

                if (phieuSuCoList.Any())
                {
                    _context.PhieuGhiNhanSuCos.RemoveRange(phieuSuCoList);
                }

                // Xóa phiếu hiện trạng nhận phòng của khách thuê
                var phieuHienTrangList = await _context.PhieuHienTrangNhanPhongs
                    .Where(p => p.MaKhachThue == hopDong.MaKhachThue)
                    .ToListAsync();

                if (phieuHienTrangList.Any())
                {
                    _context.PhieuHienTrangNhanPhongs.RemoveRange(phieuHienTrangList);
                }

                // **Xóa phiếu đăng ký dịch vụ của khách thuê**
                var phieuDangKyDichVuList = await _context.PhieuDangKyDichVus
                    .Where(p => p.MaKhachThue == hopDong.MaKhachThue)
                    .ToListAsync();

                if (phieuDangKyDichVuList.Any())
                {
                    _context.PhieuDangKyDichVus.RemoveRange(phieuDangKyDichVuList);
                }

                // Cập nhật trạng thái phòng và xóa liên kết khách thuê
                var phong = await _context.PhongTros.FindAsync(hopDong.MaPhong);
                if (phong != null)
                {
                    phong.TrangThai = "Trống";
                    phong.MaKhachThue = null;
                    _context.PhongTros.Update(phong);
                }

                // Xóa hợp đồng
                _context.HopDongs.Remove(hopDong);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }





    }
}
