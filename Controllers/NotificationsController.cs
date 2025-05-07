using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_QuanLyPhongTro.Models;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var khachThue = await _context.KhachThues
                .FirstOrDefaultAsync(k => k.Email == User.FindFirstValue(ClaimTypes.Email));
            if (khachThue == null)
                return NotFound("Không tìm thấy thông tin khách thuê");

            // Lấy tất cả thông báo chưa đọc
            var notifications = await _context.Notifications
                .Where(n => n.MaKhachThue == khachThue.MaKhachThue && !n.IsRead)  // Chỉ lấy thông báo chưa đọc
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            // Đánh dấu tất cả thông báo là đã đọc khi khách thuê mở trang
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptAppointment(int id)
        {
            var appointment = await _context.LichHen
                .Include(a => a.KhachThue) // Đảm bảo rằng khách thuê được bao gồm trong dữ liệu
                .FirstOrDefaultAsync(a => a.MaLichHen == id);

            if (appointment == null)
                return Json(new { success = false, message = "Lịch hẹn không tồn tại." });

            appointment.TrangThai = "Accepted"; // Cập nhật trạng thái lịch hẹn
            _context.LichHen.Update(appointment);

            // Thêm thông báo cho khách thuê
            var notification = new Notification
            {
                MaKhachThue = appointment.KhachThue.MaKhachThue,
                Message = $"Lịch hẹn cho phòng {appointment.PhongTro.SoPhong} đã được chấp nhận.",
                CreatedAt = DateTime.Now,
                IsRead = false // Đánh dấu thông báo là chưa đọc
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.LichHen
                .Include(a => a.KhachThue) // Đảm bảo rằng khách thuê được bao gồm trong dữ liệu
                .FirstOrDefaultAsync(a => a.MaLichHen == id);

            if (appointment == null)
                return Json(new { success = false, message = "Lịch hẹn không tồn tại." });

            appointment.TrangThai = "Rejected"; // Cập nhật trạng thái lịch hẹn
            _context.LichHen.Update(appointment);

            // Thêm thông báo cho khách thuê
            var notification = new Notification
            {
                MaKhachThue = appointment.KhachThue.MaKhachThue,
                Message = $"Lịch hẹn cho phòng {appointment.PhongTro.SoPhong} đã bị từ chối.",
                CreatedAt = DateTime.Now,
                IsRead = false // Đánh dấu thông báo là chưa đọc
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}