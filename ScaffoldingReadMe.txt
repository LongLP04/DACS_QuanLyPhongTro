using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DACS_QuanLyPhongTro.Controllers
{
    public class HopDongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HopDongController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Xác nhận hợp đồng
        [HttpGet]
        [Authorize(Roles = "KhachThue")]
        public async Task<IActionResult> XacNhan(int maHopDong)
        {
            var hopDong = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.MaHopDong == maHopDong);

            if (hopDong == null) return NotFound("Không tìm thấy hợp đồng.");

            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (currentUserEmail != hopDong.KhachThue?.Email)
            {
                return Forbid("Bạn không có quyền xác nhận hợp đồng này.");
            }

            // Lưu MaHopDong vào Session
            HttpContext.Session.SetInt32("MaHopDong", hopDong.MaHopDong);

            return View(hopDong);
        }

        // POST: Xác nhận hợp đồng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "KhachThue")]
        public async Task<IActionResult> XacNhan(int maHopDong, bool dongY)
        {
            var hopDong = await _context.HopDongs
                .Include(h => h.KhachThue)
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.MaHopDong == maHopDong);

            if (hopDong == null) return NotFound("Không tìm thấy hợp đồng.");

            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (currentUserEmail != hopDong.KhachThue?.Email)
            {
                return Forbid("Bạn không có quyền xác nhận hợp đồng này.");
            }

            if (dongY)
            {
                hopDong.TrangThai = "Đã Xác Nhận";
                var phongTro = await _context.PhongTros
                    .FirstOrDefaultAsync(p => p.MaPhong == hopDong.MaPhong);
                if (phongTro != null)
                {
                    phongTro.TrangThai = "Đã Thuê";
                    _context.Update(phongTro);
                }

                TempData["ThongBao"] = "Bạn đã xác nhận hợp đồng thành công. Trạng thái phòng đã được cập nhật.";

                // Xóa MaHopDong khỏi Session sau khi xác nhận
                HttpContext.Session.Remove("MaHopDong");
            }
            else
            {
                TempData["ThongBao"] = "Bạn đã từ chối xác nhận hợp đồng.";
                HttpContext.Session.Remove("MaHopDong");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}




@model DACS_QuanLyPhongTro.Models.HopDong
@{
    ViewData["Title"] = "Xác Nhận Hợp Đồng";
    Layout = "~/Views/Shared/_Layout.cshtml"; // Điều chỉnh layout nếu cần
}

<div class="container mt-5">
    <h2>Xác Nhận Hợp Đồng</h2>

    @if (TempData["ThongBao"] != null)
    {
        <div class="alert alert-success alert-dismissible fade show" role="alert">
            @TempData["ThongBao"]
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">×</span>
            </button>
        </div>
    }

    <div class="card">
        <div class="card-body">
            <h5 class="card-title">Thông Tin Hợp Đồng</h5>
            <p><strong>Mã Hợp Đồng:</strong> @Model.MaHopDong</p>
            <p><strong>Khách Thuê:</strong> @Model.KhachThue?.HoTen</p>
            <p><strong>Phòng Trọ:</strong> @Model.PhongTro?.SoPhong</p>
            <p><strong>Ngày Bắt Đầu:</strong> @Model.NgayBatDau.ToString("dd/MM/yyyy")</p>
            <p><strong>Ngày Kết Thúc:</strong> @Model.NgayKetThuc.ToString("dd/MM/yyyy")</p>
            <p><strong>Tiền Cọc:</strong> @Model.TienCoc.ToString("N0") VNĐ</p>
            <p><strong>Trạng Thái:</strong> @Model.TrangThai</p>
        </div>
    </div>

    <form asp-action="XacNhan" method="post">
        @Html.AntiForgeryToken()
        <input type="hidden" name="maHopDong" value="@Model.MaHopDong" />

        <div class="form-group mt-3">
            <label>Bạn có muốn xác nhận hợp đồng này không?</label>
            <div>
                <input type="radio" name="dongY" value="true" required /> Có
                <input type="radio" name="dongY" value="false" /> Không
            </div>
        </div>

        <div class="form-group">
            <input type="submit" value="Xác Nhận" class="btn btn-primary" />
            <a asp-action="Index" asp-controller="Home" class="btn btn-secondary ml-2">Quay Lại</a>
        </div>
    </form>
</div>

@section Scripts {
    <script>
        $(document).ready(function () {
            $('.alert').alert();
        });
    </script>
}





@if (maHopDong.HasValue)
{
    <a asp-controller="HopDong"
       asp-action="XacNhan"
       asp-route-maHopDong="@maHopDong.Value"
       class="nav-item nav-link">
        Xác nhận hợp đồng
    </a>
}