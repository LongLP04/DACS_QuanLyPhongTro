using Microsoft.AspNetCore.Mvc;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
