using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class PhongTroController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PhongTroController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<int?> GetMaChuTroAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return null;
            }

            var chuTro = await _context.ChuTros
                .FirstOrDefaultAsync(c => c.Email == user.Email);
            if (chuTro == null)
            {
                return null;
            }

            return chuTro.MaChuTro;
        }

        // GET: ChuTroArea/PhongTro
        public async Task<IActionResult> Index()
        {
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                TempData["ErrorMessage"] = "Không thể xác thực chủ trọ.";
                return Unauthorized();
            }

            var phongTros = await _context.PhongTros
                .Include(p => p.ToaNha)
                .Include(p => p.KhachThue)
                .Where(p => p.ToaNha.MaChuTro == maChuTro)
                .ToListAsync();

            return View(phongTros);
        }

        // GET: ChuTroArea/PhongTro/Create
        public async Task<IActionResult> Create()
        {
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                TempData["ErrorMessage"] = "Không thể xác thực chủ trọ.";
                return Unauthorized();
            }

            var toaNhas = await _context.ToaNhas
                .Where(t => t.MaChuTro == maChuTro)
                .Select(t => new SelectListItem
                {
                    Value = t.MaToaNha.ToString(),
                    Text = t.TenToaNha
                })
                .ToListAsync();

            ViewBag.MaToaNha = toaNhas;
            return View();
        }

        // POST: ChuTroArea/PhongTro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SoPhong,Tang,DienTich,GiaThue,TrangThai,MoTa,MaToaNha,MaKhachThue")] PhongTro phongTro, IFormFile Hinhanh)
        {
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                TempData["ErrorMessage"] = "Không thể xác thực chủ trọ.";
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra MaToaNha thuộc chủ trọ
                    var toaNha = await _context.ToaNhas
                        .FirstOrDefaultAsync(t => t.MaToaNha == phongTro.MaToaNha && t.MaChuTro == maChuTro);
                    if (toaNha == null)
                    {
                        TempData["ErrorMessage"] = "Tòa nhà không hợp lệ hoặc không thuộc quyền quản lý.";
                        return View(phongTro);
                    }

                    // Xử lý upload hình ảnh
                    if (Hinhanh != null && Hinhanh.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var extension = Path.GetExtension(Hinhanh.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            TempData["ErrorMessage"] = "Chỉ hỗ trợ các định dạng hình ảnh .jpg, .jpeg, .png.";
                            return View(phongTro);
                        }

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Hinhanh.CopyToAsync(stream);
                        }
                        phongTro.Hinhanh = "/images/" + fileName;
                    }

                    _context.PhongTros.Add(phongTro);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Phòng trọ đã được thêm thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thêm phòng trọ. Vui lòng thử lại.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường.";
            }

            // Load lại danh sách tòa nhà
            var toaNhas = await _context.ToaNhas
                .Where(t => t.MaChuTro == maChuTro)
                .Select(t => new SelectListItem
                {
                    Value = t.MaToaNha.ToString(),
                    Text = t.TenToaNha
                })
                .ToListAsync();
            ViewBag.MaToaNha = toaNhas;

            return View(phongTro);
        }

        // GET: ChuTroArea/PhongTro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID phòng trọ không hợp lệ.";
                return NotFound();
            }

            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                TempData["ErrorMessage"] = "Không thể xác thực chủ trọ.";
                return Unauthorized();
            }

            var phongTro = await _context.PhongTros
                .Include(p => p.ToaNha)
                .FirstOrDefaultAsync(p => p.MaPhong == id && p.ToaNha.MaChuTro == maChuTro);
            if (phongTro == null)
            {
                TempData["ErrorMessage"] = "Phòng trọ không tồn tại hoặc không thuộc quyền quản lý.";
                return NotFound();
            }

            var toaNhas = await _context.ToaNhas
                .Where(t => t.MaChuTro == maChuTro)
                .Select(t => new SelectListItem
                {
                    Value = t.MaToaNha.ToString(),
                    Text = t.TenToaNha
                })
                .ToListAsync();
            ViewBag.MaToaNha = toaNhas;

            return View(phongTro);
        }

        // POST: ChuTroArea/PhongTro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaPhong,SoPhong,Tang,DienTich,GiaThue,TrangThai,MoTa,MaToaNha,MaKhachThue")] PhongTro phongTro, IFormFile Hinhanh)
        {
            if (id != phongTro.MaPhong)
            {
                TempData["ErrorMessage"] = "ID phòng trọ không khớp.";
                return NotFound();
            }

            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                TempData["ErrorMessage"] = "Không thể xác thực chủ trọ.";
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPhongTro = await _context.PhongTros
                        .Include(p => p.ToaNha)
                        .FirstOrDefaultAsync(p => p.MaPhong == id && p.ToaNha.MaChuTro == maChuTro);
                    if (existingPhongTro == null)
                    {
                        TempData["ErrorMessage"] = "Phòng trọ không tồn tại hoặc không thuộc quyền quản lý.";
                        return NotFound();
                    }

                    // Kiểm tra MaToaNha mới
                    var toaNha = await _context.ToaNhas
                        .FirstOrDefaultAsync(t => t.MaToaNha == phongTro.MaToaNha && t.MaChuTro == maChuTro);
                    if (toaNha == null)
                    {
                        TempData["ErrorMessage"] = "Tòa nhà không hợp lệ hoặc không thuộc quyền quản lý.";
                        return View(phongTro);
                    }

                    // Xử lý upload hình ảnh
                    if (Hinhanh != null && Hinhanh.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var extension = Path.GetExtension(Hinhanh.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            TempData["ErrorMessage"] = "Chỉ hỗ trợ các định dạng hình ảnh .jpg, .jpeg, .png.";
                            return View(phongTro);
                        }

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Hinhanh.CopyToAsync(stream);
                        }
                        // Xóa hình cũ nếu có
                        if (!string.IsNullOrEmpty(existingPhongTro.Hinhanh))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingPhongTro.Hinhanh.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        existingPhongTro.Hinhanh = "/images/" + fileName;
                    }

                    // Cập nhật các trường
                    existingPhongTro.SoPhong = phongTro.SoPhong;
                    existingPhongTro.Tang = phongTro.Tang;
                    existingPhongTro.DienTich = phongTro.DienTich;
                    existingPhongTro.GiaThue = phongTro.GiaThue;
                    existingPhongTro.TrangThai = phongTro.TrangThai;
                    existingPhongTro.MoTa = phongTro.MoTa;
                    existingPhongTro.MaToaNha = phongTro.MaToaNha;
                    existingPhongTro.MaKhachThue = phongTro.MaKhachThue;

                    _context.Update(existingPhongTro);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Phòng trọ đã được cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật phòng trọ. Vui lòng thử lại.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường.";
            }

            // Load lại danh sách tòa nhà
            var toaNhas = await _context.ToaNhas
                .Where(t => t.MaChuTro == maChuTro)
                .Select(t => new SelectListItem
                {
                    Value = t.MaToaNha.ToString(),
                    Text = t.TenToaNha
                })
                .ToListAsync();
            ViewBag.MaToaNha = toaNhas;

            return View(phongTro);
        }

        // GET: ChuTroArea/PhongTro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID phòng trọ không hợp lệ.";
                return NotFound();
            }

            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                TempData["ErrorMessage"] = "Không thể xác thực chủ trọ.";
                return Unauthorized();
            }

            var phongTro = await _context.PhongTros
                .Include(p => p.ToaNha)
                .Include(p => p.KhachThue)
                .Include(p => p.ChiSoDienNuocs)
                .FirstOrDefaultAsync(p => p.MaPhong == id && p.ToaNha.MaChuTro == maChuTro);
            if (phongTro == null)
            {
                TempData["ErrorMessage"] = "Phòng trọ không tồn tại hoặc không thuộc quyền quản lý.";
                return NotFound();
            }

            return View(phongTro);
        }

        // POST: ChuTroArea/PhongTro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maChuTro = await GetMaChuTroAsync();
            if (maChuTro == null)
            {
                TempData["ErrorMessage"] = "Không thể xác thực chủ trọ.";
                return Unauthorized();
            }

            var phongTro = await _context.PhongTros
                .Include(p => p.ToaNha)
                .Include(p => p.KhachThue)
                .Include(p => p.ChiSoDienNuocs)
                .FirstOrDefaultAsync(p => p.MaPhong == id && p.ToaNha.MaChuTro == maChuTro);
            if (phongTro == null)
            {
                TempData["ErrorMessage"] = "Phòng trọ không tồn tại hoặc không thuộc quyền quản lý.";
                return NotFound();
            }

            try
            {
                // Kiểm tra nếu phòng trọ có khách thuê hoặc chỉ số điện nước
                if (phongTro.KhachThue != null || phongTro.ChiSoDienNuocs.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa phòng trọ vì vẫn còn khách thuê hoặc chỉ số điện nước liên quan.";
                    return View("Delete", phongTro);
                }

                // Xóa hình ảnh nếu có
                if (!string.IsNullOrEmpty(phongTro.Hinhanh))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, phongTro.Hinhanh.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.PhongTros.Remove(phongTro);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Phòng trọ đã được xóa thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa phòng trọ. Vui lòng thử lại.";
                return View("Delete", phongTro);
            }
        }
    }
}