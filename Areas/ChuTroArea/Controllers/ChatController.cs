using DACS_QuanLyPhongTro.Models;
using DACS_QuanLyPhongTro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ChuTroArea/Chat
        public async Task<IActionResult> Index(string? userId = null, string? hotenkhach = null)
        {
            var user = await _userManager.GetUserAsync(User);
            // Lấy danh sách khách thuê thuộc các phòng trọ mà chủ trọ này sở hữu
            var khachThueList = await _context.PhongTros
                .Include(p => p.KhachThue)
                .ThenInclude(k => k.ApplicationUser)
                .Include(p => p.ToaNha)
                .ThenInclude(t => t.ChuTro)
                .Where(p => p.KhachThue != null &&
                    ((p.ToaNha.ChuTro.ApplicationUserId != null && p.ToaNha.ChuTro.ApplicationUserId == user.Id)
                    || (!string.IsNullOrEmpty(user.Email) && p.ToaNha.ChuTro.Email == user.Email)))
                .Select(p => p.KhachThue)
                .Distinct()
                .ToListAsync();

            ViewData["SelectedUserId"] = userId;
            ViewData["SelectedUserName"] = hotenkhach;

            return View(khachThueList);
        }

        [HttpGet]
        public async Task<IActionResult> GetTenantList()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Không xác định được chủ trọ." });
            }

            var tenantRooms = await _context.PhongTros
                .Include(p => p.KhachThue)
                    .ThenInclude(k => k.ApplicationUser)
                .Include(p => p.ToaNha)
                    .ThenInclude(t => t.ChuTro)
                .Where(p => p.KhachThue != null
                            && (
                                (p.ToaNha.ChuTro.ApplicationUserId != null && p.ToaNha.ChuTro.ApplicationUserId == user.Id)
                                || (!string.IsNullOrEmpty(user.Email) && p.ToaNha.ChuTro.Email == user.Email)
                            ))
                .Select(p => new ChatTenantViewModel
                {
                    UserId = p.KhachThue.ApplicationUserId ?? string.Empty,
                    TenantName = p.KhachThue.HoTen,
                    Phone = p.KhachThue.SoDienThoai,
                    RoomLabel = $"Phòng {p.SoPhong}"
                })
                .ToListAsync();

            var tenants = tenantRooms
                .Where(t => !string.IsNullOrWhiteSpace(t.UserId))
                .GroupBy(t => t.UserId)
                .Select(g => g.First())
                .OrderBy(t => t.TenantName)
                .ToList();

            return Json(new { success = true, data = tenants });
        }

        // API: Lấy lịch sử chat giữa chủ trọ và khách thuê
        [HttpGet]
        public async Task<IActionResult> GetChatHistory(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == userId) ||
                            (m.SenderId == userId && m.ReceiverId == currentUser.Id))
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    m.SenderId,
                    m.ReceiverId,
                    m.Content,
                    m.Timestamp
                })
                .ToListAsync();
            return Json(messages);
        }
    }
}
