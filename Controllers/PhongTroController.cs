using DACS_QuanLyPhongTro.Models;
using DACS_QuanLyPhongTro.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DACS_QuanLyPhongTro.Controllers
{
    public class PhongTroController : Controller
    {
        private readonly IPhongTroRepository _phongTroRepository;
        private readonly ApplicationDbContext _context;

        public PhongTroController(IPhongTroRepository phongTroRepository, ApplicationDbContext context)
        {
            _phongTroRepository = phongTroRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm, decimal? minPrice, decimal? maxPrice, decimal? minArea, decimal? maxArea)
        {
            var query = _context.PhongTros
                .Include(p => p.ToaNha)
                    .ThenInclude(t => t.ChuTro)
                .AsQueryable();

            // Áp dụng các điều kiện lọc trên database trước
            if (minPrice.HasValue)
                query = query.Where(p => p.GiaThue >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.GiaThue <= maxPrice.Value);
            if (minArea.HasValue)
                query = query.Where(p => p.DienTich >= minArea.Value);
            if (maxArea.HasValue)
                query = query.Where(p => p.DienTich <= maxArea.Value);

            // Lấy tạm danh sách (chỉ lấy các cột cần thiết hoặc toàn bộ)
            var list = await query.ToListAsync();

            // Nếu có searchTerm, lọc tiếp trên client (bộ nhớ)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var terms = searchTerm.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Lọc theo từng từ khóa, bắt đầu với toàn bộ danh sách
                foreach (var term in terms)
                {
                    list = list.Where(p =>
                        p.ToaNha.DiaChi.ToLower()
                        .Split(new char[] { ' ', ',', '.', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Contains(term)
                    ).ToList();
                }
            }

            ViewBag.TotalProducts = list.Count;

            // Tạo lại danh sách khoảng giá, khoảng diện tích theo list mới (client side)
            var priceRanges = new List<object>
    {
        new { Range = new { Min = 0m, Max = 2000000m }, Count = list.Count(p => p.GiaThue >= 0m && p.GiaThue <= 2000000m) },
        new { Range = new { Min = 2000000m, Max = 4000000m }, Count = list.Count(p => p.GiaThue > 2000000m && p.GiaThue <= 4000000m) },
        new { Range = new { Min = 4000000m, Max = 6000000m }, Count = list.Count(p => p.GiaThue > 4000000m && p.GiaThue <= 6000000m) },
        new { Range = new { Min = 6000000m, Max = (decimal?)null }, Count = list.Count(p => p.GiaThue > 6000000m) }
    };
            ViewBag.PriceRanges = priceRanges;

            var areaRanges = new List<object>
    {
        new { Range = new { Min = 0m, Max = 20m }, Count = list.Count(p => p.DienTich >= 0m && p.DienTich <= 20m) },
        new { Range = new { Min = 20m, Max = 40m }, Count = list.Count(p => p.DienTich > 20m && p.DienTich <= 40m) },
        new { Range = new { Min = 40m, Max = 60m }, Count = list.Count(p => p.DienTich > 40m && p.DienTich <= 60m) },
        new { Range = new { Min = 60m, Max = (decimal?)null }, Count = list.Count(p => p.DienTich > 60m) }
    };
            ViewBag.AreaRanges = areaRanges;

            return View(list);
        }
        public async Task<IActionResult> Details(int id)
        {
            var phongTro = await _phongTroRepository.GetByIdAsync(id);
            if (phongTro == null)
            {
                return NotFound();
            }
            return View(phongTro);
        }
        [Authorize(Roles = "KhachThue")]
        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentRequest request)
        {
            try
            {
                // Kiểm tra phòng trọ tồn tại
                var phongTro = await _context.PhongTros.FindAsync(request.MaPhong);
                if (phongTro == null)
                    return Json(new { success = false, message = "Phòng trọ không tồn tại" });

                // Lấy thông tin khách thuê dựa trên email người dùng
                var khachThue = await _context.KhachThues
                    .FirstOrDefaultAsync(k => k.Email == User.FindFirstValue(ClaimTypes.Email));
                if (khachThue == null)
                    return Json(new { success = false, message = "Thông tin khách thuê không hợp lệ" });

                // Kiểm tra lịch hẹn trùng
                var existingAppointment = await _context.LichHen.AnyAsync(a =>
                    a.MaPhong == request.MaPhong &&
                    a.NgayHen == DateTime.Parse(request.NgayHen) &&
                    a.GioHen == TimeSpan.Parse(request.GioHen) &&
                    a.TrangThai != "Rejected");
                if (existingAppointment)
                    return Json(new { success = false, message = "Lịch hẹn này đã được đặt!" });

                // Tạo lịch hẹn mới
                var appointment = new LichHen
                {
                    MaPhong = request.MaPhong,
                    MaKhachThue = khachThue.MaKhachThue,
                    NgayHen = DateTime.Parse(request.NgayHen),
                    GioHen = TimeSpan.Parse(request.GioHen),
                    GhiChu = request.GhiChu,
                    TrangThai = "Pending"
                };

                _context.LichHen.Add(appointment);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        // Action trả view giao diện tìm phòng gần tôi
        public IActionResult TimPhongGanToi()
        {
            return View();
        }

        // API trả JSON danh sách phòng trọ gần vị trí lat,lng
        [HttpGet]
        public IActionResult GetPhongGanToi(double lat, double lng)
        {
            var phongGanToi = _context.PhongTros
                .Include(p => p.ToaNha)
                .Where(p => !string.IsNullOrEmpty(p.ToaNha.ViTri))
                .AsEnumerable()  // Chuyển sang LINQ to Objects để dùng hàm tính khoảng cách
                .Select(p =>
                {
                    var parts = p.ToaNha.ViTri.Split(',');
                    if (parts.Length != 2) return null;

                    if (!double.TryParse(parts[0], out double lat2) || !double.TryParse(parts[1], out double lng2)) return null;

                    double distance = GetDistanceCosine(lat, lng, lat2, lng2);

                    return new
                    {
                        p.MaPhong,
                        p.SoPhong,
                        TenToaNha = p.ToaNha.TenToaNha,
                        p.GiaThue,
                        DiaChi = p.ToaNha.DiaChi,
                        ViTri = p.ToaNha.ViTri,
                        Distance = distance
                    };
                })
                .Where(x => x != null)
                .OrderBy(x => x.Distance)
                .Take(20)
                .ToList();

            return Json(phongGanToi);
        }

        // Hàm tính khoảng cách giữa 2 tọa độ (km) theo công thức cosine
        private double GetDistanceCosine(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Bán kính Trái Đất (km)
            double d = Math.Acos(
                Math.Sin(ToRadians(lat1)) * Math.Sin(ToRadians(lat2)) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Cos(ToRadians(lon2 - lon1))
            ) * R;
            return d;
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }



    }


    public class AppointmentRequest
    {
        public int MaPhong { get; set; }
        public string NgayHen { get; set; }
        public string GioHen { get; set; }
        public string GhiChu { get; set; }
    }
}