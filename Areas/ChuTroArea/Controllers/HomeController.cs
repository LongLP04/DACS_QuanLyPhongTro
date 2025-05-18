using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

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
            return View();
        }

    }
}
