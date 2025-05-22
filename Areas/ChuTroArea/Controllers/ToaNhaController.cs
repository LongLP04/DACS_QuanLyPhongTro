using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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
            _logger.LogInformation("Index action called.");
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                _logger.LogError("Unauthorized access: MaChuTro is null.");
                return Unauthorized();
            }

            var toaNhas = await _context.ToaNhas
                .Where(t => t.MaChuTro == maChuTro)
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
            catch (DbUpdateException ex)
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
}