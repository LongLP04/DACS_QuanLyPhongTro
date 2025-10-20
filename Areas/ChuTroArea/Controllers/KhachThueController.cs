using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace DACS_QuanLyPhongTro.Areas.ChuTroArea.Controllers
{
    [Area("ChuTroArea")]
    [Authorize(Roles = "ChuTro")]
    public class KhachThueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<KhachThueController> _logger;

        public KhachThueController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<KhachThueController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: ChuTroArea/KhachThue
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var chuTro = await _context.ChuTros.FirstOrDefaultAsync(c => c.Email == user.Email);
            if (chuTro == null)
            {
                return Unauthorized();
            }


            // Get all rooms that have a tenant and belong to this ChuTro, include tenant and building
            var roomsWithTenants = await _context.PhongTros
                .Include(p => p.KhachThue)
                .Include(p => p.ToaNha)
                .Where(p => p.MaKhachThue != null && p.ToaNha.MaChuTro == chuTro.MaChuTro)
                .ToListAsync();

            var vm = roomsWithTenants.Select(p => new TenantRow
            {
                MaKhachThue = p.MaKhachThue ?? 0,
                HoTen = p.KhachThue?.HoTen,
                SoDienThoai = p.KhachThue?.SoDienThoai,
                ApplicationUserId = p.KhachThue?.ApplicationUserId,
                Email = p.KhachThue?.Email,
                SoPhong = p.SoPhong,
                Tang = p.Tang,
                TenToaNha = p.ToaNha?.TenToaNha,
                DiaChi = p.ToaNha?.DiaChi,
                TrangThaiPhong = p.TrangThai
            }).ToList();

            // Set title and ChuTro display name for layout
            ViewData["Title"] = "Quản lý khách thuê";
            ViewData["ChuTroHoTen"] = chuTro.HoTen ?? chuTro.Email;

            return View(vm);
        }

        // GET: ChuTroArea/KhachThue/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var chuTro = await _context.ChuTros.FirstOrDefaultAsync(c => c.Email == user.Email);
            if (chuTro == null) return Unauthorized();

            var kh = await _context.KhachThues
                .Include(k => k.PhongTros)
                    .ThenInclude(p => p.ToaNha)
                .Include(k => k.HopDongs)
                    .ThenInclude(h => h.PhongTro)
                        .ThenInclude(p => p.ToaNha)
                .Include(k => k.PhieuDangKyDichVus)
                    .ThenInclude(d => d.ChiTietPhieuDangKyDichVus)
                .Include(k => k.HoaDons)
                .FirstOrDefaultAsync(k => k.MaKhachThue == id);

            if (kh == null) return NotFound();

            // ensure tenant belongs to this chuTro via at least one room
            var owns = kh.PhongTros.Any(p => p.ToaNha != null && p.ToaNha.MaChuTro == chuTro.MaChuTro);
            if (!owns) return Unauthorized();

            var phong = kh.PhongTros.FirstOrDefault(p => p.ToaNha != null && p.ToaNha.MaChuTro == chuTro.MaChuTro);

            // Ensure invoices are loaded explicitly from the DB (defensive in case navigation property isn't populated)
            var hoaDons = await _context.HoaDons
                .Where(hd => hd.MaKhachThue == kh.MaKhachThue)
                .OrderByDescending(hd => hd.NgayLap)
                .ToListAsync();

            var vm = new KhachThueDetailsViewModel
            {
                MaKhachThue = kh.MaKhachThue,
                HoTen = kh.HoTen,
                Email = kh.Email,
                SoDienThoai = kh.SoDienThoai,
                CCCD = kh.CCCD,
                Phong = phong == null ? null : new PhongInfo { MaPhong = phong.MaPhong, SoPhong = phong.SoPhong, Tang = phong.Tang, TenToaNha = phong.ToaNha?.TenToaNha, DiaChi = phong.ToaNha?.DiaChi },
                HopDongs = kh.HopDongs.OrderByDescending(h => h.NgayLap).ToList(),
                PhieuDichVus = kh.PhieuDangKyDichVus.OrderByDescending(d => d.NgayBatDau).ToList(),
                HoaDons = hoaDons
            };

            ViewData["Title"] = "Chi tiết khách thuê";
            return View(vm);
        }

        public class TenantRow
        {
            public int MaKhachThue { get; set; }
            public string? HoTen { get; set; }
            public string? SoDienThoai { get; set; }
            public string? Email { get; set; }
            public string? SoPhong { get; set; }
            public int Tang { get; set; }
            public string? TenToaNha { get; set; }
            public string? DiaChi { get; set; }
            public string? TrangThaiPhong { get; set; }
            public string? ApplicationUserId { get; set; }
        }

        // ViewModel for Details
        public class KhachThueDetailsViewModel
        {
            public int MaKhachThue { get; set; }
            public string HoTen { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string SoDienThoai { get; set; } = string.Empty;
            public string CCCD { get; set; } = string.Empty;
            public PhongInfo? Phong { get; set; }
            public List<HopDong> HopDongs { get; set; } = new List<HopDong>();
            public List<PhieuDangKyDichVu> PhieuDichVus { get; set; } = new List<PhieuDangKyDichVu>();
            public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        }

        public class PhongInfo
        {
            public int MaPhong { get; set; }
            public string SoPhong { get; set; } = string.Empty;
            public int Tang { get; set; }
            public string? TenToaNha { get; set; }
            public string? DiaChi { get; set; }
        }
    }
}
