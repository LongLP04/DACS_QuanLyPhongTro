using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DACS_QuanLyPhongTro.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize(Roles = "KhachThue")]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Identity/Chat
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            // Lấy phòng trọ của khách thuê này
            var phongTro = await _context.PhongTros
                .Include(p => p.ToaNha)
                .ThenInclude(t => t.ChuTro)
                .FirstOrDefaultAsync(p => p.MaKhachThue != null && p.KhachThue.ApplicationUserId == user.Id);
            string landlordId = null;
            if (phongTro != null && phongTro.ToaNha?.ChuTro?.ApplicationUserId != null)
            {
                landlordId = phongTro.ToaNha.ChuTro.ApplicationUserId;
            }
            ViewBag.LandlordId = landlordId;
            return View();
        }

        // API: Lấy lịch sử chat giữa khách thuê và chủ trọ
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
