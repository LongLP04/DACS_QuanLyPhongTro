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
            var hopDongs = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .ToListAsync();

            return View(hopDongs);
        }

        // GET: Tạo hợp đồng
        public async Task<IActionResult> Create()
        {
            // Lấy thông tin chủ trọ hiện tại dựa trên email
            var currentChuTroEmail = User.Identity.Name; // Giả định User.Identity.Name là email
            var currentChuTro = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);

            if (currentChuTro == null)
            {
                return NotFound("Không tìm thấy thông tin chủ trọ.");
            }

            // Lấy danh sách phòng trọ thuộc các tòa nhà của chủ trọ
            var phongTros = currentChuTro.ToaNhas
                .SelectMany(t => t.PhongTros)
                .ToList();

            ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");
            return View();
        }

        // POST: Tạo hợp đồng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HopDong hopDong, string emailKhachThue)
        {
            // Kiểm tra email khách thuê
            var khachThue = await _context.KhachThues.FirstOrDefaultAsync(k => k.Email == emailKhachThue);
            if (khachThue == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy khách thuê với email này.");
                var currentChuTroEmail = User.Identity.Name;
                var currentChuTro = await _context.ChuTros
                    .Include(c => c.ToaNhas)
                    .ThenInclude(t => t.PhongTros)
                    .FirstOrDefaultAsync(c => c.Email == currentChuTroEmail);
                var phongTros = currentChuTro.ToaNhas
                    .SelectMany(t => t.PhongTros)
                    .Where(p => p.TrangThai == "Trống")
                    .ToList();
                ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");
                return View(hopDong);
            }

            // Kiểm tra xem phòng có thuộc chủ trọ không và có trống không
            var currentChuTroEmailCheck = User.Identity.Name;
            var currentChuTroCheck = await _context.ChuTros
                .Include(c => c.ToaNhas)
                .ThenInclude(t => t.PhongTros)
                .FirstOrDefaultAsync(c => c.Email == currentChuTroEmailCheck);

            var phongTro = currentChuTroCheck.ToaNhas
                .SelectMany(t => t.PhongTros)
                .FirstOrDefault(p => p.MaPhong == hopDong.MaPhong);

            if (phongTro == null)
            {
                ModelState.AddModelError("MaPhong", "Phòng này không thuộc về bạn hoặc không tồn tại.");
                var phongTros = currentChuTroCheck.ToaNhas
                    .SelectMany(t => t.PhongTros)
                    .Where(p => p.TrangThai == "Trống")
                    .ToList();
                ViewBag.PhongTros = new SelectList(phongTros, "MaPhong", "SoPhong");
                return View(hopDong);
            }

            if (phongTro.TrangThai != "Trống")
            {
                ModelState.AddModelError("MaPhong", "Phòng này hiện không trống.");
                var phongTros = currentChuTroCheck.ToaNhas
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
    }
}