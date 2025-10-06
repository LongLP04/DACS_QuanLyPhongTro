using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    [Authorize(Roles = "KhachThue")]
    public class PhongtrocuatoiController : Controller
    {
        // Hiển thị giao diện chat của KHÁCH THUÊ với CHỦ TRỌ
        public async Task<IActionResult> IndexChat()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid("Bạn chưa đăng nhập.");

            var khachThue = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.ApplicationUserId == userId);

            if (khachThue == null)
                return NotFound("Không tìm thấy thông tin khách thuê.");

            // Lấy hợp đồng đã xác nhận của khách thuê
            var hopDong = await _context.HopDongs
                .Where(h => h.MaKhachThue == khachThue.MaKhachThue && h.TrangThai == "Đã Xác Nhận")
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ToaNha)
                        .ThenInclude(t => t.ChuTro)
                .FirstOrDefaultAsync();

            if (hopDong == null)
                return NotFound("Không tìm thấy hợp đồng đã xác nhận.");

            var chuTro = hopDong.PhongTro?.ToaNha?.ChuTro;
            if (chuTro == null)
                return NotFound("Không tìm thấy chủ trọ.");

            // Gửi chủ trọ qua View (để khách thuê chat với người này)
            var chuTroList = new List<ChuTro> { chuTro };

            return View("IndexChat", chuTroList);
        }

        [HttpGet]
    public async Task<IActionResult> GetChatHistory(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("Thiếu userId.");

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == null)
            return Unauthorized("Chưa đăng nhập.");

        var messages = await _context.Messages
            .Where(m =>
                (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                (m.SenderId == userId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.Timestamp)
            .Select(m => new
            {
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                content = m.Content,
                timestamp = m.Timestamp.ToString("dd/MM/yyyy HH:mm")
            })
            .ToListAsync();

        return Json(messages);
    }

        private readonly ApplicationDbContext _context;

        public PhongtrocuatoiController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (userId == null)
        //        return Forbid("Bạn chưa đăng nhập.");

        //    var khachThue = await _context.KhachThues
        //        .FirstOrDefaultAsync(k => k.ApplicationUserId == userId);

        //    if (khachThue == null)
        //        return NotFound("Không tìm thấy thông tin khách thuê.");

        //    var hopDongs = await _context.HopDongs
        //        .Where(h => h.MaKhachThue == khachThue.MaKhachThue && h.TrangThai == "Đã Xác Nhận")
        //        .Include(h => h.PhongTro)
        //            .ThenInclude(p => p.ToaNha)
        //                .ThenInclude(t => t.ChuTro)
        //        .ToListAsync();

        //    var phongTros = hopDongs
        //        .Select(h => h.PhongTro)
        //        .Where(p => p != null)
        //        .ToList();

        //    return View(phongTros);
        //}
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Forbid("Bạn chưa đăng nhập.");

            var khachThue = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.ApplicationUserId == userId);

            if (khachThue == null)
                return NotFound("Không tìm thấy thông tin khách thuê.");

            var hopDong = await _context.HopDongs
                .Where(h => h.MaKhachThue == khachThue.MaKhachThue && h.TrangThai == "Đã Xác Nhận")
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.ToaNha)
                        .ThenInclude(t => t.ChuTro)
                .FirstOrDefaultAsync();

            if (hopDong == null)
                return View(null);

            return View(hopDong.PhongTro);
        }


    }
}
