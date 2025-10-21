using System.Security.Claims;
using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
        [Area("ChuTroArea")]
        [Authorize(Roles = "ChuTro")]
        public class ChiSoDienNuocController : Controller
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

            public ChiSoDienNuocController(ApplicationDbContext context) 
            {
                _context = context;
            }

    // GET: Danh sách chỉ số điện nước
    // viewMode: "all" shows all readings; "latest" shows latest reading per room (with fake record if none)
    public async Task<IActionResult> Index(string viewMode = "all")
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

            var currentChuTroEmail = User.Identity.Name; // Lấy email của chủ trọ đang đăng nhập
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null)
            {
                return NotFound("Không tìm thấy thông tin chủ trọ.");
            }

            // Lấy danh sách phòng đã thuê
            var phongTrosDaThue = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();

            var phongIds = phongTrosDaThue.Select(p => p.MaPhong).ToList();

            ViewBag.ViewMode = viewMode ?? "all";

            if (viewMode == "latest")
            {
                var allChiSo = await _context.ChiSoDienNuocs
                    .Include(c => c.PhongTro)
                    .Where(c => phongIds.Contains(c.MaPhong))
                    .ToListAsync();

                var latestPerRoom = allChiSo
                    .GroupBy(c => c.MaPhong)
                    .Select(g => g.OrderByDescending(x => x.NgayGhi).First())
                    .ToList();

                var result = new List<ChiSoDienNuoc>();
                foreach (var phong in phongTrosDaThue)
                {
                    var chiSo = latestPerRoom.FirstOrDefault(c => c.MaPhong == phong.MaPhong);
                    if (chiSo != null)
                    {
                        result.Add(chiSo);
                    }
                    else
                    {
                        result.Add(new ChiSoDienNuoc
                        {
                            MaChiSo = 0,
                            PhongTro = phong,
                            MaPhong = phong.MaPhong,
                            NgayGhi = DateTime.MinValue,
                        });
                    }
                }

                return View(result);
            }

            var chiSoDienNuocs = await _context.ChiSoDienNuocs
                .Include(c => c.PhongTro)
                .Where(c => phongIds.Contains(c.MaPhong))
                .OrderByDescending(c => c.NgayGhi)
                .ToListAsync();

            return View(chiSoDienNuocs);
        }

        // GET: Ghi chỉ số điện nước
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

            var phongTrosDaThue = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();

            if (!phongTrosDaThue.Any())
            {
                TempData["ThongBao"] = "Hiện không có phòng nào đang được thuê.";
                return RedirectToAction("Index");
            }

            ViewBag.PhongTros = new SelectList(phongTrosDaThue, "MaPhong", "SoPhong");
            Console.WriteLine($"ViewBag.PhongTros count: {phongTrosDaThue.Count}");
            return View();
        }

        // POST: Ghi chỉ số điện nước
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Backwards-compatible overload (reads ReadingMonth from form if caller posts without it)
        public async Task<IActionResult> Create(ChiSoDienNuoc chiSoDienNuoc)
        {
            var readingMonth = Request.Form["ReadingMonth"].FirstOrDefault();
            return await Create(chiSoDienNuoc, readingMonth);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChiSoDienNuoc chiSoDienNuoc, string? ReadingMonth)
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

            // Kiểm tra và lấy thông tin phòng
            var phongTro = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .FirstOrDefault(p => p.MaPhong == chiSoDienNuoc.MaPhong);

            if (phongTro == null)
            {
                ModelState.AddModelError("MaPhong", "Phòng này không thuộc về bạn hoặc không tồn tại.");
            }
            else if (phongTro.TrangThai != "Đã Thuê")
            {
                ModelState.AddModelError("MaPhong", "Phòng này hiện không được thuê.");
            }
            else
            {
                // Gán PhongTro để tránh lỗi "The PhongTro field is required"
                chiSoDienNuoc.PhongTro = phongTro;
            }

            // Log dữ liệu nhận được để debug
            Console.WriteLine($"Received: MaPhong={chiSoDienNuoc.MaPhong}, ChiSoDienCu={chiSoDienNuoc.ChiSoDienCu}, " +
                             $"ChiSoDienMoi={chiSoDienNuoc.ChiSoDienMoi}, ChiSoNuocCu={chiSoDienNuoc.ChiSoNuocCu}, " +
                             $"ChiSoNuocMoi={chiSoDienNuoc.ChiSoNuocMoi}, DonGiaDien={chiSoDienNuoc.DonGiaDien}, " +
                             $"DonGiaNuoc={chiSoDienNuoc.DonGiaNuoc}");

            // Kiểm tra các trường bắt buộc thủ công
            if (chiSoDienNuoc.ChiSoDienCu <= 0 || chiSoDienNuoc.ChiSoDienMoi <= 0 || chiSoDienNuoc.ChiSoNuocCu <= 0 || chiSoDienNuoc.ChiSoNuocMoi <= 0)
            {
                ModelState.AddModelError("", "Chỉ số điện và nước phải lớn hơn 0.");
            }
            if (chiSoDienNuoc.ChiSoDienMoi < chiSoDienNuoc.ChiSoDienCu)
            {
                ModelState.AddModelError("ChiSoDienMoi", "Chỉ số điện mới phải lớn hơn chỉ số điện cũ.");
            }
            if (chiSoDienNuoc.ChiSoNuocMoi < chiSoDienNuoc.ChiSoNuocCu)
            {
                ModelState.AddModelError("ChiSoNuocMoi", "Chỉ số nước mới phải lớn hơn chỉ số nước cũ.");
            }
            if (chiSoDienNuoc.DonGiaDien <= 0 || chiSoDienNuoc.DonGiaNuoc <= 0)
            {
                ModelState.AddModelError("", "Đơn giá điện và nước phải lớn hơn 0.");
            }

            var phongTrosDaThue = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();
            ViewBag.PhongTros = new SelectList(phongTrosDaThue, "MaPhong", "SoPhong");

            if (ModelState.ErrorCount > 0)
            {
                return View(chiSoDienNuoc);
            }

            try
            {
                // parse reading month if provided
                DateTime monthStart = DateTime.Now;
                DateTime monthEnd = DateTime.Now;
                if (!string.IsNullOrEmpty(ReadingMonth) && DateTime.TryParse(ReadingMonth + "-01", out var parsed))
                {
                    monthStart = new DateTime(parsed.Year, parsed.Month, 1);
                    monthEnd = monthStart.AddMonths(1).AddTicks(-1);
                }

                // check duplicate for same MaPhong in the same month
                var exists = await _context.ChiSoDienNuocs
                    .AnyAsync(c => c.MaPhong == chiSoDienNuoc.MaPhong && c.NgayGhi >= monthStart && c.NgayGhi <= monthEnd);

                if (exists)
                {
                    ModelState.AddModelError("", "Đã tồn tại chỉ số cho phòng này trong tháng đã chọn. Vui lòng chỉnh sửa bản ghi hiện có.");
                    return View(chiSoDienNuoc);
                }

                // Set NgayGhi to the end of the selected month to mark the reading's billing month
                chiSoDienNuoc.NgayGhi = monthEnd;
                _context.ChiSoDienNuocs.Add(chiSoDienNuoc);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "Chỉ số điện nước đã được ghi thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi lưu dữ liệu: {ex.Message}");
                return View(chiSoDienNuoc);
            }
        }

        // GET: Chi tiết chỉ số điện nước
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var chiSoDienNuoc = await _context.ChiSoDienNuocs
                .Include(c => c.PhongTro)
                .FirstOrDefaultAsync(c => c.MaChiSo == id);

            if (chiSoDienNuoc == null)
                return NotFound();

            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null || !currentChuTro.ToaNhas.SelectMany(t => t.PhongTros).Any(p => p.MaPhong == chiSoDienNuoc.MaPhong))
            {
                return Forbid("Bạn không có quyền xem thông tin này.");
            }

            return View(chiSoDienNuoc);
        }
        // GET: Chỉnh sửa chỉ số điện nước
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var chiSoDienNuoc = await _context.ChiSoDienNuocs
                .Include(c => c.PhongTro)
                .FirstOrDefaultAsync(c => c.MaChiSo == id);

            if (chiSoDienNuoc == null)
                return NotFound();

            // Kiểm tra chỉ số đã được in ra hóa đơn chưa
            bool daCoHoaDon = await _context.HoaDons.AnyAsync(hd => hd.MaChiSo == chiSoDienNuoc.MaChiSo);
            if (daCoHoaDon)
            {
                TempData["Error"] = "Chỉ số này đã được in ra hóa đơn và được xác nhận bởi khách thuê, vui lòng không sửa !";
                return RedirectToAction("Index");
            }

            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null || !currentChuTro.ToaNhas.SelectMany(t => t.PhongTros).Any(p => p.MaPhong == chiSoDienNuoc.MaPhong))
            {
                return Forbid("Bạn không có quyền chỉnh sửa thông tin này.");
            }

            var phongTrosDaThue = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();

            ViewBag.PhongTros = new SelectList(phongTrosDaThue, "MaPhong", "SoPhong", chiSoDienNuoc.MaPhong);

            return View(chiSoDienNuoc);
        }

        // POST: Chỉnh sửa chỉ số điện nước
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChiSoDienNuoc chiSoDienNuoc)
        {
            if (id != chiSoDienNuoc.MaChiSo)
                return NotFound();

            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null || !currentChuTro.ToaNhas.SelectMany(t => t.PhongTros).Any(p => p.MaPhong == chiSoDienNuoc.MaPhong))
            {
                return Forbid("Bạn không có quyền chỉnh sửa thông tin này.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra phòng thuộc chủ trọ và đang thuê
                    var phongTro = currentChuTro.ToaNhas
                        .SelectMany(t => t.PhongTros)
                        .FirstOrDefault(p => p.MaPhong == chiSoDienNuoc.MaPhong && p.TrangThai == "Đã Thuê");

                    if (phongTro == null)
                    {
                        ModelState.AddModelError("MaPhong", "Phòng không hợp lệ hoặc không được thuê.");
                        var phongTros = currentChuTro.ToaNhas
                            .SelectMany(t => t.PhongTros)
                            .Where(p => p.TrangThai == "Đã Thuê")
                            .ToList();
                        ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong", chiSoDienNuoc.MaPhong);
                        return View(chiSoDienNuoc);
                    }

                    chiSoDienNuoc.PhongTro = phongTro;

                    _context.Update(chiSoDienNuoc);
                    await _context.SaveChangesAsync();

                    TempData["ThongBao"] = "Chỉ số điện nước đã được cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ChiSoDienNuocs.Any(e => e.MaChiSo == chiSoDienNuoc.MaChiSo))
                        return NotFound();
                    else
                        throw;
                }
            }

            var phongTrosDaThue = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .Where(p => p.TrangThai == "Đã Thuê")
                .ToList();

            ViewBag.PhongTros = new SelectList(phongTrosDaThue, "MaPhong", "SoPhong", chiSoDienNuoc.MaPhong);
            return View(chiSoDienNuoc);
        }


        // GET: Xác nhận xóa chỉ số điện nước
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var chiSoDienNuoc = await _context.ChiSoDienNuocs
                .Include(c => c.PhongTro)
                .FirstOrDefaultAsync(c => c.MaChiSo == id);

            if (chiSoDienNuoc == null)
                return NotFound();

            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null || !currentChuTro.ToaNhas.SelectMany(t => t.PhongTros).Any(p => p.MaPhong == chiSoDienNuoc.MaPhong))
            {
                return Forbid("Bạn không có quyền xóa thông tin này.");
            }

            return View(chiSoDienNuoc);
        }

        // POST: Thực hiện xóa chỉ số điện nước
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiSoDienNuoc = await _context.ChiSoDienNuocs.FindAsync(id);
            if (chiSoDienNuoc == null)
                return NotFound();

            var currentChuTroEmail = User.Identity.Name;
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null || !currentChuTro.ToaNhas.SelectMany(t => t.PhongTros).Any(p => p.MaPhong == chiSoDienNuoc.MaPhong))
            {
                return Forbid("Bạn không có quyền xóa thông tin này.");
            }

            _context.ChiSoDienNuocs.Remove(chiSoDienNuoc);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Chỉ số điện nước đã được xóa thành công.";
            return RedirectToAction(nameof(Index));
        }

    }
}