using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class ToaNhaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ToaNhaController> _logger;

        public ToaNhaController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ToaNhaController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: ChuTroArea/ToaNha/RoomsByBuilding/5
        public async Task<IActionResult> RoomsByBuilding(int id)
        {
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                _logger.LogWarning("Unauthorized access attempt to RoomsByBuilding. MaChuTro null.");
                return Unauthorized();
            }

            var toaNha = await _context.ToaNhas
                .Include(t => t.PhongTros)
                .FirstOrDefaultAsync(t => t.MaToaNha == id && t.MaChuTro == maChuTro.Value);

            if (toaNha == null)
            {
                _logger.LogWarning("RoomsByBuilding: ToaNha not found or does not belong to current ChuTro. Id={Id}, MaChuTro={MaChuTro}", id, maChuTro);
                return NotFound();
            }

            var vm = new DACS_QuanLyPhongTro.Models.ViewModels.RoomsByBuildingViewModel
            {
                MaToaNha = toaNha.MaToaNha,
                TenToaNha = toaNha.TenToaNha,
                DiaChi = toaNha.DiaChi
            };

            var grouped = new Dictionary<int, List<DACS_QuanLyPhongTro.Models.ViewModels.PhongTroSummary>>();

            var roomsOrdered = toaNha.PhongTros.OrderByDescending(p => p.Tang).ToList();
            foreach (var p in roomsOrdered)
            {
                // find related entities
                var latestHopDong = await _context.HopDongs
                    .Where(h => h.MaPhong == p.MaPhong)
                    .OrderByDescending(h => h.NgayKetThuc)
                    .FirstOrDefaultAsync();

                var latestChiSo = await _context.ChiSoDienNuocs
                    .Where(c => c.MaPhong == p.MaPhong)
                    .OrderByDescending(c => c.NgayGhi)
                    .FirstOrDefaultAsync();

                DACS_QuanLyPhongTro.Models.HoaDon? latestHoaDon = null;
                if (latestChiSo != null)
                {
                    latestHoaDon = await _context.HoaDons
                        .Where(hd => hd.MaChiSo == latestChiSo.MaChiSo)
                        .OrderByDescending(hd => hd.NgayLap)
                        .FirstOrDefaultAsync();
                }

                var summary = new DACS_QuanLyPhongTro.Models.ViewModels.PhongTroSummary
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    GiaThue = p.GiaThue,
                    TrangThai = p.TrangThai,
                    Hinhanh = p.Hinhanh,
                    KhachThueName = p.KhachThue != null ? p.KhachThue.HoTen : string.Empty,
                    KhachThueApplicationUserId = p.KhachThue != null ? p.KhachThue.ApplicationUserId : null,
                    MaHopDong = latestHopDong?.MaHopDong,
                    LatestChiSoId = latestChiSo?.MaChiSo,
                    LatestHoaDonId = latestHoaDon?.MaHoaDon
                };

                if (!grouped.ContainsKey(p.Tang)) grouped[p.Tang] = new List<DACS_QuanLyPhongTro.Models.ViewModels.PhongTroSummary>();
                grouped[p.Tang].Add(summary);
            }

            vm.Floors = grouped;

            return View("RoomsByBuilding", vm);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPassword([FromBody] PasswordVerificationModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Người dùng không hợp lệ." });
                }

                if (string.IsNullOrEmpty(model?.Password))
                {
                    return Json(new { success = false, message = "Password is required." });
                }
                var result = await _userManager.CheckPasswordAsync(user, model.Password);
                if (result)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Mật khẩu không đúng." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        private async Task<int?> GetMaChuTroAsync()
        {
            _logger.LogInformation("Getting MaChuTro for current user.");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User is null. User not authenticated.");
                return null;
            }

            var chuTro = await _context.ChuTros
                .FirstOrDefaultAsync(c => c.Email == user.Email);
            if (chuTro == null)
            {
                _logger.LogWarning("No ChuTro found for user with email: {Email}", user.Email);
                return null;
            }

            _logger.LogInformation("Found MaChuTro: {MaChuTro} for user: {UserId}", chuTro.MaChuTro, user.Id);
            return chuTro.MaChuTro;
        }

        // GET: ChuTroArea/ToaNha
        public async Task<IActionResult> Index()
        {
            string hoTen = "Chủ trọ";

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
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

            _logger.LogInformation("Index action called.");
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                _logger.LogError("Unauthorized access: MaChuTro is null.");
                return Unauthorized();
            }

            var toaNhas = await _context.ToaNhas
                .Where(t => t.MaChuTro == maChuTro)
                .Include(t => t.PhongTros)
                .Include(t => t.ChuTro)
                .ToListAsync();
            
            _logger.LogInformation("Retrieved {Count} ToaNhas for MaChuTro: {MaChuTro}", toaNhas.Count, maChuTro);
            return View(toaNhas);
        }

        // GET: ChuTroArea/ToaNha/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Create GET action called.");
            return View();
        }

        // POST: ChuTroArea/ToaNha/Create
        // POST: ChuTroArea/ToaNha/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToaNha toaNha)
        {
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                return Unauthorized();
            }

            // Nếu không có tọa độ vị trí, gán mặc định
            if (string.IsNullOrEmpty(toaNha.ViTri))
            {
                toaNha.ViTri = "10.823,106.627"; // Tọa độ TP.HCM mặc định
            }

            toaNha.MaChuTro = maChuTro.Value;

            _context.ToaNhas.Add(toaNha);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        // GET: ChuTroArea/ToaNha/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogInformation("Edit GET action called with id: {Id}", id);
            if (id == null)
            {
                _logger.LogWarning("Id is null.");
                return NotFound();
            }

            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                _logger.LogError("Unauthorized access: MaChuTro is null.");
                return Unauthorized();
            }

            var toaNha = await _context.ToaNhas
                .FirstOrDefaultAsync(t => t.MaToaNha == id && t.MaChuTro == maChuTro);
            if (toaNha == null)
            {
                _logger.LogWarning("ToaNha not found with id: {Id} for MaChuTro: {MaChuTro}", id, maChuTro);
                return NotFound();
            }

            return View(toaNha);
        }

        // POST: ChuTroArea/ToaNha/Edit/5
        // POST: ChuTroArea/ToaNha/Edit/5
        // POST: ChuTroArea/ToaNha/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ToaNha toaNha)
        {
            if (id != toaNha.MaToaNha)
            {
                return NotFound();
            }

            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                return Unauthorized();
            }

            try
            {
                var existingToaNha = await _context.ToaNhas
                    .FirstOrDefaultAsync(t => t.MaToaNha == id && t.MaChuTro == maChuTro);
                if (existingToaNha == null)
                {
                    return NotFound();
                }

                // Cập nhật các trường
                existingToaNha.TenToaNha = toaNha.TenToaNha;
                existingToaNha.DiaChi = toaNha.DiaChi;
                existingToaNha.TongSoTang = toaNha.TongSoTang;
                existingToaNha.MoTa = toaNha.MoTa;
                existingToaNha.ViTri = toaNha.ViTri; // Cập nhật vị trí

                _context.Update(existingToaNha);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật tòa nhà.");
            }

            return View(toaNha);
        }


        // GET: ChuTroArea/ToaNha/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation("Delete GET action called with id: {Id}", id);
            if (id == null)
            {
                _logger.LogWarning("Id is null.");
                return NotFound();
            }

            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                _logger.LogError("Unauthorized access: MaChuTro is null.");
                return Unauthorized();
            }

            var toaNha = await _context.ToaNhas
                .FirstOrDefaultAsync(t => t.MaToaNha == id && t.MaChuTro == maChuTro);
            if (toaNha == null)
            {
                _logger.LogWarning("ToaNha not found with id: {Id} for MaChuTro: {MaChuTro}", id, maChuTro);
                return NotFound();
            }

            return View(toaNha);
        }

        // POST: ChuTroArea/ToaNha/Delete/5
        // POST: ChuTroArea/ToaNha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("DeleteConfirmed POST action called with id: {Id}", id);
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                _logger.LogError("Unauthorized access: MaChuTro is null.");
                return Unauthorized();
            }

            var toaNha = await _context.ToaNhas
                .FirstOrDefaultAsync(t => t.MaToaNha == id && t.MaChuTro == maChuTro);
            if (toaNha == null)
            {
                _logger.LogWarning("ToaNha not found with id: {Id} for MaChuTro: {MaChuTro}", id, maChuTro);
                return NotFound();
            }

            // Kiểm tra nếu tòa nhà có phòng đã đăng ký
            var hasRooms = await _context.PhongTros.AnyAsync(p => p.MaToaNha == id);
            if (hasRooms)
            {
                // Nếu có phòng, không cho phép xóa và thông báo
                TempData["ErrorMessage"] = "Không thể xóa tòa nhà này vì có phòng trọ đã đăng ký. Vui lòng xóa phòng trước.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.ToaNhas.Remove(toaNha);
                await _context.SaveChangesAsync();
                _logger.LogInformation("ToaNha deleted successfully: {ToaNhaName}", toaNha.TenToaNha);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting ToaNha: {ToaNhaName}", toaNha.TenToaNha);
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi xóa tòa nhà.");
                return View(toaNha);
            }
        }

    }

    public class PasswordVerificationModel
    {
        public string? Password { get; set; }
    }
}