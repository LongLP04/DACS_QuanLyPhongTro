using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_QuanLyPhongTro.Models;
using DACS_QuanLyPhongTro.Controllers;
using System.Security.Claims;

[Area("ChuTroArea")]
[Authorize(Roles = "ChuTro")] // Chỉ cho phép ChuTro truy cập
public class LichHenController : Controller
{
    private readonly ApplicationDbContext _context;

    public LichHenController(ApplicationDbContext context)
    {
       _context = context;
    }

    public async Task<IActionResult> Appointments()
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
        var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var appointments = await _context.LichHen
            .Include(a => a.PhongTro)
                .ThenInclude(p => p.ToaNha)
                    .ThenInclude(t => t.ChuTro)
            .Where(a => a.PhongTro.ToaNha.ChuTro.ApplicationUserId == appUserId)
            .Include(a => a.KhachThue)
            .ToListAsync();

        return View(appointments);
    }


    public class AppointmentRequest
    {
        public int Id { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> AcceptAppointment([FromBody] AppointmentRequest request)
    {
        try
        {
            if (request?.Id <= 0)
            {
                return Json(new { success = false, message = "ID lịch hẹn không hợp lệ." });
            }

            var appointment = await _context.LichHen
                .Include(a => a.KhachThue)
                .Include(a => a.PhongTro) // Đảm bảo phòng cũng được bao gồm
                .FirstOrDefaultAsync(a => a.MaLichHen == request.Id);

            if (appointment == null)
            {
                return Json(new { success = false, message = "Lịch hẹn không tồn tại." });
            }

            // Cập nhật trạng thái lịch hẹn
            appointment.TrangThai = "Accepted";
            _context.LichHen.Update(appointment);

            // Tạo thông báo cho khách thuê
            var notification = new Notification
            {
                MaKhachThue = appointment.KhachThue.MaKhachThue,
                Message = $"Lịch hẹn cho phòng {appointment.PhongTro.SoPhong} đã được chấp nhận.",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            // Thêm thông báo vào bảng Notifications
            _context.Notifications.Add(notification);

            // Lưu các thay đổi vào cơ sở dữ liệu
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Json(new { success = true, message = "Lịch hẹn đã được chấp nhận và thông báo đã được lưu." });
            }
            else
            {
                return Json(new { success = false, message = "Không có thay đổi nào được lưu vào cơ sở dữ liệu." });
            }
        }
        catch (Exception ex)
        {
            // Ghi lại lỗi chi tiết vào log hoặc console để kiểm tra
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return Json(new { success = false, message = "Có lỗi xảy ra trong quá trình xử lý." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RejectAppointment([FromBody] AppointmentRequest request)
    {
        try
        {
            var appointment = await _context.LichHen
                .Include(a => a.KhachThue)
                .Include(a => a.PhongTro) // Đảm bảo rằng phòng được bao gồm
                .FirstOrDefaultAsync(a => a.MaLichHen == request.Id);

            if (appointment == null)
            {
                return Json(new { success = false, message = "Lịch hẹn không tồn tại." });
            }

            // Cập nhật trạng thái lịch hẹn
            appointment.TrangThai = "Rejected";
            _context.LichHen.Update(appointment);

            // Tạo thông báo cho khách thuê
            var notification = new Notification
            {
                MaKhachThue = appointment.KhachThue.MaKhachThue,
                Message = $"Lịch hẹn cho phòng {appointment.PhongTro.SoPhong} đã bị từ chối.",
                CreatedAt = DateTime.Now,
                IsRead = false // Đánh dấu thông báo là chưa đọc
            };

            // Thêm thông báo vào bảng Notifications
            _context.Notifications.Add(notification);

            // Lưu các thay đổi vào cơ sở dữ liệu
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Json(new { success = true, message = "Lịch hẹn đã bị từ chối và thông báo đã được lưu." });
            }
            else
            {
                return Json(new { success = false, message = "Không có thay đổi nào được lưu vào cơ sở dữ liệu." });
            }
        }
        catch (Exception ex)
        {
            // Ghi lại lỗi chi tiết vào log hoặc console để kiểm tra
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return Json(new { success = false, message = "Có lỗi xảy ra trong quá trình xử lý." });
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteAppointment([FromBody] AppointmentRequest request)
    {
        try
        {
            if (request?.Id <= 0)
                return Json(new { success = false, message = "ID lịch hẹn không hợp lệ." });

            var appointment = await _context.LichHen
                .FirstOrDefaultAsync(a => a.MaLichHen == request.Id);

            if (appointment == null)
                return Json(new { success = false, message = "Lịch hẹn không tồn tại." });

            _context.LichHen.Remove(appointment);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return Json(new { success = true, message = "Lịch hẹn đã được xóa thành công." });
            else
                return Json(new { success = false, message = "Không có thay đổi nào được lưu vào cơ sở dữ liệu." });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return Json(new { success = false, message = "Có lỗi xảy ra trong quá trình xử lý." });
        }
    }


}