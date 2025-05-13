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

        public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice, decimal? minArea, decimal? maxArea, string? trangThai, string? address)
        {
            var query = _context.PhongTros
                .Include(p => p.ToaNha)
                    .ThenInclude(t => t.ChuTro)
                .AsQueryable();

            // Tổng số phòng trọ
            ViewBag.TotalProducts = await query.CountAsync();

            // Tạo danh sách khoảng giá (price ranges)
            var priceRanges = new List<object>
    {
        new { Range = new { Min = 0m, Max = 2000000m }, Count = await query.CountAsync(p => p.GiaThue >= 0m && p.GiaThue <= 2000000m) },
        new { Range = new { Min = 2000000m, Max = 4000000m }, Count = await query.CountAsync(p => p.GiaThue > 2000000m && p.GiaThue <= 4000000m) },
        new { Range = new { Min = 4000000m, Max = 6000000m }, Count = await query.CountAsync(p => p.GiaThue > 4000000m && p.GiaThue <= 6000000m) },
        new { Range = new { Min = 6000000m, Max = (decimal?)null }, Count = await query.CountAsync(p => p.GiaThue > 6000000m) }
    };
            ViewBag.PriceRanges = priceRanges;

            // Tạo danh sách khoảng diện tích (area ranges) với kiểu decimal
            var areaRanges = new List<object>
    {
        new { Range = new { Min = 0m, Max = 20m }, Count = await query.CountAsync(p => p.DienTich >= 0m && p.DienTich <= 20m) },
        new { Range = new { Min = 20m, Max = 40m }, Count = await query.CountAsync(p => p.DienTich > 20m && p.DienTich <= 40m) },
        new { Range = new { Min = 40m, Max = 60m }, Count = await query.CountAsync(p => p.DienTich > 40m && p.DienTich <= 60m) },
        new { Range = new { Min = 60m, Max = (decimal?)null }, Count = await query.CountAsync(p => p.DienTich > 60m) }
    };
            ViewBag.AreaRanges = areaRanges;

            // Tạo danh sách trạng thái
            var trangThaiList = new List<string> { "Trống", "Đã thuê" };
            ViewBag.TrangThaiList = trangThaiList;

            // Tạo danh sách địa chỉ
            var addressList = await query.Select(p => p.ToaNha.DiaChi).Distinct().ToListAsync();
            ViewBag.AddressList = addressList;

            // Lọc theo giá
            if (minPrice.HasValue)
                query = query.Where(p => p.GiaThue >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.GiaThue <= maxPrice.Value);

            // Lọc theo diện tích (sử dụng decimal)
            if (minArea.HasValue)
                query = query.Where(p => p.DienTich >= minArea.Value);

            if (maxArea.HasValue)
                query = query.Where(p => p.DienTich <= maxArea.Value);

            // Lọc theo trạng thái (chỉ lấy 1 giá trị từ radio)
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(p => p.TrangThai == trangThai);

            // Lọc theo địa chỉ (chỉ lấy 1 giá trị từ radio)
            if (!string.IsNullOrEmpty(address))
                query = query.Where(p => p.ToaNha.DiaChi.Contains(address));

            var result = await query.ToListAsync();
            return View(result);
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
    }
    public class AppointmentRequest
    {
        public int MaPhong { get; set; }
        public string NgayHen { get; set; }
        public string GioHen { get; set; }
        public string GhiChu { get; set; }
    }
}