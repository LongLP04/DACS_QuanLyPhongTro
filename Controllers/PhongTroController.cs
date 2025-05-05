using DACS_QuanLyPhongTro.Models;
using DACS_QuanLyPhongTro.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice, decimal? minArea, decimal? maxArea, string? trangThai)
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

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(p => p.TrangThai == trangThai);

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
    }
}