using System.Security.Claims;
using DACS_QuanLyPhongTro.Models;
using DACS_QuanLyPhongTro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
        [Area("ChuTroArea")]
        [Authorize(Roles = "ChuTro")]
        public class HoaDonController : Controller
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

            public HoaDonController(ApplicationDbContext context) 
            {
                _context = context;
            }

            private async Task<List<RoomDropdownOptionViewModel>> BuildRoomDropdownAsync(IEnumerable<PhongTro> phongTros)
            {
                var roomList = phongTros?.Where(p => p != null).ToList() ?? new List<PhongTro>();
                if (!roomList.Any())
                {
                    return new List<RoomDropdownOptionViewModel>();
                }

                var roomIds = roomList.Select(p => p.MaPhong).ToList();
                var invoiceRaw = await _context.HoaDons
                    .Where(h => roomIds.Contains(h.MaPhong))
                    .Select(h => new
                    {
                        h.MaPhong,
                        MonthDate = h.ChiSoDienNuoc != null ? h.ChiSoDienNuoc.NgayGhi : h.NgayLap
                    })
                    .ToListAsync();

                var invoiceLookup = invoiceRaw
                    .GroupBy(x => x.MaPhong)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.MonthDate.ToString("yyyy-MM")).Distinct().ToList()
                    );

                return roomList
                    .Select(room =>
                    {
                        invoiceLookup.TryGetValue(room.MaPhong, out var recordedInvoices);
                        return new RoomDropdownOptionViewModel
                        {
                            MaPhong = room.MaPhong,
                            SoPhong = room.SoPhong,
                            TenantName = room.KhachThue?.HoTen ?? "Chưa có khách",
                            RecordedInvoiceMonths = recordedInvoices ?? new List<string>()
                        };
                    })
                    .OrderBy(option => option.SoPhong)
                    .ToList();
            }

        // GET: Xem danh sách hóa đơn của các phòng trọ
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

            //if (!phongTrosDaThue.Any())
            //{
            //    return NotFound("Không có phòng nào đang được thuê.");
            //}

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
        public async Task<IActionResult> Create()
        {
            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .ThenInclude(p => p.KhachThue)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null)
            {
                return NotFound("Không tìm thấy thông tin chủ trọ.");
            }

            var phongTros = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();

            if (!phongTros.Any())
            {
                TempData["ThongBao"] = "Hiện không có phòng nào đang được thuê để tạo hóa đơn.";
                return RedirectToAction("Index");
            }

            ViewBag.RoomOptions = await BuildRoomDropdownAsync(phongTros);
            ViewBag.SelectedBillingMonth = DateTime.Now.ToString("yyyy-MM");

            return View();
        }


        // Action xử lý khi người dùng submit form
        [HttpPost]
        public async Task<IActionResult> Create(HoaDon hoaDon, string? BillingMonth)
        {
            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .ThenInclude(p => p.KhachThue)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null)
            {
                return NotFound("Không tìm thấy thông tin chủ trọ.");
            }

            var phongTros = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();

            if (!phongTros.Any())
            {
                TempData["ThongBao"] = "Hiện không có phòng nào đang được thuê để tạo hóa đơn.";
                return RedirectToAction("Index");
            }

            ViewBag.RoomOptions = await BuildRoomDropdownAsync(phongTros);

            var billingMonthValue = BillingMonth;
            if (string.IsNullOrWhiteSpace(billingMonthValue))
            {
                billingMonthValue = Request.Form["BillingMonth"].FirstOrDefault();
            }
            if (string.IsNullOrWhiteSpace(billingMonthValue))
            {
                billingMonthValue = DateTime.Now.ToString("yyyy-MM");
            }
            ViewBag.SelectedBillingMonth = billingMonthValue;
            BillingMonth = billingMonthValue;

            // parse billing month (format yyyy-MM from <input type=month>)
            DateTime periodStart = DateTime.Now;
            DateTime periodEnd = DateTime.Now;
            if (!string.IsNullOrEmpty(BillingMonth) && DateTime.TryParse(BillingMonth + "-01", out var parsed))
            {
                periodStart = new DateTime(parsed.Year, parsed.Month, 1);
                periodEnd = periodStart.AddMonths(1).AddTicks(-1);
            }

            // KIỂM TRA: Đã tồn tại hóa đơn của phòng này trong tháng này chưa?
            var existedBill = _context.HoaDons
                .Include(h => h.ChiSoDienNuoc)
                .ThenInclude(c => c.PhongTro)
                .Any(h => h.ChiSoDienNuoc.PhongTro.MaPhong == hoaDon.MaPhong
                          && h.ChiSoDienNuoc.NgayGhi >= periodStart
                          && h.ChiSoDienNuoc.NgayGhi <= periodEnd);
            if (existedBill)
            {
                ModelState.AddModelError("", "Hóa đơn tháng này cho phòng đã được tạo. Vui lòng không tạo trùng.");
                return View(hoaDon);
            }

            // Tìm chỉ số của phòng trong tháng hóa đơn
            var chiSoDienNuoc = _context.ChiSoDienNuocs
                .Where(c => c.MaPhong == hoaDon.MaPhong && c.NgayGhi >= periodStart && c.NgayGhi <= periodEnd)
                .OrderByDescending(c => c.NgayGhi)
                .FirstOrDefault();

            if (chiSoDienNuoc == null)
            {
                ModelState.AddModelError("", "Không tìm thấy chỉ số điện nước cho phòng này trong tháng đã chọn.");
                return View(hoaDon);
            }

            // Gán MaChiSo
            hoaDon.MaChiSo = chiSoDienNuoc.MaChiSo;

            // Tìm thông tin phòng và khách thuê
            var phong = phongTros.FirstOrDefault(p => p.MaPhong == hoaDon.MaPhong);

            if (phong == null)
            {
                ModelState.AddModelError("", "Phòng không tồn tại hoặc không thuộc quyền quản lý của bạn.");
                return View(hoaDon);
            }

            var khachThue = phong.KhachThue ?? _context.KhachThues.FirstOrDefault(k => k.MaKhachThue == phong.MaKhachThue);
            if (khachThue == null)
            {
                ModelState.AddModelError("", "Phòng này chưa có khách thuê.");
                return View(hoaDon);
            }

            hoaDon.MaKhachThue = khachThue.MaKhachThue;

            // Lấy TẤT CẢ phiếu dịch vụ đã xác nhận thuộc THÁNG hóa đơn (cho phép nhiều phiếu trong tháng)
            var phieuDichVus = _context.PhieuDangKyDichVus
                .Where(p => p.MaKhachThue == khachThue.MaKhachThue
                            && p.TrangThai == "Đã xác nhận"
                            && p.NgayBatDau >= periodStart
                            && p.NgayBatDau <= periodEnd)
                .Include(p => p.ChiTietPhieuDangKyDichVus)
                .ToList();

            decimal tongTienDichVu = phieuDichVus.SelectMany(p => p.ChiTietPhieuDangKyDichVus).Sum(ct => ct.TongTienDichVu);
            hoaDon.TienDichVu = tongTienDichVu;

            // Tính toán hóa đơn
            hoaDon.TienDien = chiSoDienNuoc.SoDienTieuThu * chiSoDienNuoc.DonGiaDien;
            hoaDon.TienNuoc = chiSoDienNuoc.SoNuocTieuThu * chiSoDienNuoc.DonGiaNuoc;
            hoaDon.TienPhong = phong.GiaThue;
            hoaDon.TongTien = hoaDon.TienDien + hoaDon.TienNuoc + hoaDon.TienPhong + hoaDon.TienDichVu;
            hoaDon.NgayLap = DateTime.Now;
            hoaDon.TrangThai = "Chưa thanh toán";

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CheckPrerequisites(int maPhong, string billingMonth)
        {
            var currentChuTroEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(currentChuTroEmail))
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .ThenInclude(p => p.KhachThue)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin chủ trọ." });
            }

            var phong = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .FirstOrDefault(p => p.MaPhong == maPhong);

            if (phong == null)
            {
                return Json(new { success = false, message = "Phòng không thuộc quyền quản lý của bạn." });
            }

            if (phong.MaKhachThue == null)
            {
                return Json(new
                {
                    success = true,
                    hasService = false,
                    hasReading = false,
                    tenantName = "Chưa có khách",
                    message = "Phòng này chưa có khách thuê."
                });
            }

            if (string.IsNullOrWhiteSpace(billingMonth) || !DateTime.TryParse(billingMonth + "-01", out var parsed))
            {
                return Json(new { success = false, message = "Tháng lập hóa đơn không hợp lệ." });
            }

            var periodStart = new DateTime(parsed.Year, parsed.Month, 1);
            var periodEnd = periodStart.AddMonths(1).AddTicks(-1);

            var hasReading = await _context.ChiSoDienNuocs
                .AnyAsync(c => c.MaPhong == phong.MaPhong && c.NgayGhi >= periodStart && c.NgayGhi <= periodEnd);

            var hasService = await _context.PhieuDangKyDichVus
                .AnyAsync(p => p.MaKhachThue == phong.MaKhachThue
                               && p.TrangThai == "Đã xác nhận"
                               && p.NgayBatDau >= periodStart
                               && p.NgayBatDau <= periodEnd);

            return Json(new
            {
                success = true,
                hasService,
                hasReading,
                tenantName = phong.KhachThue?.HoTen ?? "",
                message = string.Empty
            });
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
        public async Task<IActionResult> ExportPdf(int id)
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                var hoaDon = await _context.HoaDons
                    .Include(h => h.KhachThue)
                    .Include(h => h.PhongTro)
                        .ThenInclude(pt => pt.ToaNha)
                            .ThenInclude(tn => tn.ChuTro)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                    return NotFound();

                var pdfBytes = HoaDonPdfGenerator.Generate(hoaDon);
                return File(pdfBytes, "application/pdf", $"HoaDon_{hoaDon.MaHoaDon}.pdf");
            }

        // GET: HoaDon/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);
            if (hoaDon == null)
                return NotFound();
            // Có thể truyền thêm ViewBag nếu cần
            return View(hoaDon);
        }

        // POST: HoaDon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HoaDon hoaDon)
        {
            if (!ModelState.IsValid)
                return View(hoaDon);
            var existing = await _context.HoaDons.FindAsync(hoaDon.MaHoaDon);
            if (existing == null)
                return NotFound();
            // Cập nhật các trường cần thiết
            existing.TienDien = hoaDon.TienDien;
            existing.TienNuoc = hoaDon.TienNuoc;
            existing.TienPhong = hoaDon.TienPhong;
            existing.TienDichVu = hoaDon.TienDichVu;
            existing.TongTien = hoaDon.TongTien;
            existing.TrangThai = hoaDon.TrangThai;
            // ... có thể cập nhật thêm các trường khác nếu cần
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: HoaDon/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
                return NotFound();
            _context.HoaDons.Remove(hoaDon);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
