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
                var chuTroId = 0;

            if (User.Identity.IsAuthenticated)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(email))
                {
                    var chuTro = await _context.ChuTros.FirstOrDefaultAsync(c => c.Email == email);
                    if (chuTro != null)
                    {
                        hoTen = chuTro.HoTen;
                        chuTroId = chuTro.MaChuTro;
                    }
                }
            }

            // Lấy dữ liệu cho dashboard
            var dashboardData = await GetDashboardDataAsync(chuTroId);
            
            ViewData["ChuTroHoTen"] = hoTen;
            ViewData["DashboardData"] = dashboardData;
            
            return View();
        }

        private async Task<DashboardData> GetDashboardDataAsync(int chuTroId)
        {
            var data = new DashboardData();

            // Lấy danh sách tòa nhà của chủ trọ
            var toaNhas = await _context.ToaNhas
                .Where(t => t.MaChuTro == chuTroId)
                .Include(t => t.PhongTros)
                .ToListAsync();

            data.TongSoToaNha = toaNhas.Count;
            data.TongSoPhongTro = toaNhas.Sum(t => t.PhongTros.Count);
            data.TongSoPhongDaThue = toaNhas.Sum( t => t.PhongTros.Count(p => p.TrangThai == "Đã thuê"));

            // Lấy mã phòng của các tòa nhà
            var maPhongs = toaNhas.SelectMany(t => t.PhongTros.Select(p => p.MaPhong)).ToList();

            // Doanh thu tháng này
            var thangNay = DateTime.Now.Month;
            var namNay = DateTime.Now.Year;
            var doanhThuThangNay = await _context.HoaDons
                .Where(h => maPhongs.Contains(h.MaPhong) && 
                           h.NgayLap.Month == thangNay && 
                           h.NgayLap.Year == namNay)
                .SumAsync(h => h.TongTien);

            data.DoanhThuThangNay = doanhThuThangNay;

            // Doanh thu 6 tháng gần nhất
            var doanhThuTheoThang = new List<DoanhThuThang>();
            for (int i = 5; i >= 0; i--)
            {
                var thang = DateTime.Now.AddMonths(-i);
                var doanhThu = await _context.HoaDons
                    .Where(h => maPhongs.Contains(h.MaPhong) && 
                               h.NgayLap.Month == thang.Month && 
                               h.NgayLap.Year == thang.Year)
                    .SumAsync(h => h.TongTien);

                doanhThuTheoThang.Add(new DoanhThuThang
                {
                    Thang = thang.ToString("MM/yyyy"),
                    DoanhThu = doanhThu
                });
            }
            data.DoanhThuTheoThang = doanhThuTheoThang;

            // Tỷ lệ phòng trống/đã thuê
            data.TyLePhongTrong = data.TongSoPhongTro - data.TongSoPhongDaThue;
            data.TyLePhongDaThue = data.TongSoPhongDaThue;

            // Khách thuê mới 6 tháng gần nhất
            var khachThueMoiTheoThang = new List<KhachThueMoiThang>();
            for (int i = 5; i >= 0; i--)
            {
                var thang = DateTime.Now.AddMonths(-i);
                var soKhachThueMoi = await _context.HopDongs
                    .Where(h => maPhongs.Contains(h.MaPhong.Value) && 
                               h.NgayLap.Month == thang.Month && 
                               h.NgayLap.Year == thang.Year)
                    .CountAsync();

                khachThueMoiTheoThang.Add(new KhachThueMoiThang
                {
                    Thang = thang.ToString("MM/yyyy"),
                    SoKhachThueMoi = soKhachThueMoi
                });
            }
            data.KhachThueMoiTheoThang = khachThueMoiTheoThang;

            // Sự cố theo tháng - Lấy qua KhachThue thuộc các phòng của chủ trọ
            var suCoTheoThang = new List<SuCoThang>();
            
            // Lấy danh sách khách thuê thuộc các phòng của chủ trọ
            var maKhachThues = await _context.PhongTros
                .Where(p => maPhongs.Contains(p.MaPhong) && p.MaKhachThue.HasValue)
                .Select(p => p.MaKhachThue.Value)
                .ToListAsync();
                
            for (int i = 5; i >= 0; i--)
            {
                var thang = DateTime.Now.AddMonths(-i);
                var soSuCo = await _context.PhieuGhiNhanSuCos
                    .Where(s => maKhachThues.Contains(s.MaKhachThue) && 
                               s.NgayGhiNhan.Month == thang.Month && 
                               s.NgayGhiNhan.Year == thang.Year)
                    .CountAsync();

                suCoTheoThang.Add(new SuCoThang
                {
                    Thang = thang.ToString("MM/yyyy"),
                    SoSuCo = soSuCo
                });
            }
            data.SuCoTheoThang = suCoTheoThang;

            return data;
        }
    }

    // Classes để chứa dữ liệu dashboard
    public class DashboardData
    {
        public int TongSoToaNha { get; set; }
        public int TongSoPhongTro { get; set; }
        public int TongSoPhongDaThue { get; set; }
        public decimal DoanhThuThangNay { get; set; }
        public int TyLePhongTrong { get; set; }
        public int TyLePhongDaThue { get; set; }
        public List<DoanhThuThang> DoanhThuTheoThang { get; set; } = new List<DoanhThuThang>();
        public List<KhachThueMoiThang> KhachThueMoiTheoThang { get; set; } = new List<KhachThueMoiThang>();
        public List<SuCoThang> SuCoTheoThang { get; set; } = new List<SuCoThang>();
    }

    public class DoanhThuThang
    {
        public string Thang { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class KhachThueMoiThang
    {
        public string Thang { get; set; }
        public int SoKhachThueMoi { get; set; }
    }

    public class SuCoThang
    {
        public string Thang { get; set; }
        public int SoSuCo { get; set; }
    }
}
