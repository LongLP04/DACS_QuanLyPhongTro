
using Microsoft.AspNetCore.Mvc;
using DACS_QuanLyPhongTro.Models;
using DACS_QuanLyPhongTro.Services;

namespace DACS_QuanLyPhongTro.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ApplicationDbContext _db;
        public PaymentController(IVnPayService vnPayService, ApplicationDbContext db)
        {
            _vnPayService = vnPayService;
            _db = db;
        }

        // Tạo link thanh toán VNPay cho hóa đơn
        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(int invoiceId)
        {
            var invoice = _db.HoaDons.Find(invoiceId);
            if (invoice == null)
                return NotFound();

            var model = new PaymentInformationModel
            {
                OrderType = "billpayment",
                Amount = (double)invoice.TongTien,
                OrderDescription = $"Thanh toan hoa don #{invoiceId}",
                Name = "KhachThue"
            };
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        // Xử lý callback từ VNPay sau khi thanh toán
        // [HttpGet]
        // public IActionResult PaymentCallbackVnpay()
        // {
        //     var response = _vnPayService.PaymentExecute(Request.Query);
        //     if (response.Success)
        //     {
        //         // Thanh toán thành công, cập nhật trạng thái hóa đơn
        //         var invoice = _db.HoaDons.FirstOrDefault(h => h.MaHoaDon.ToString() == response.OrderId);
        //         if (invoice != null)
        //         {
        //             invoice.TrangThai = "Đã thanh toán";
        //             _db.SaveChanges();
        //         }
        //         return Content("Thanh toán thành công!");
        //     }
        //     return Content("Thanh toán thất bại!");
        // }
        [HttpGet]
public IActionResult PaymentCallbackVnpay()
{
    var response = _vnPayService.PaymentExecute(Request.Query);
    var vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();

    if (response.Success && response.VnPayResponseCode == "00")
    {
        var invoice = _db.HoaDons.FirstOrDefault(h => h.MaHoaDon.ToString() == vnp_TxnRef);
        if (invoice != null)
        {
            // 1. Cập nhật trạng thái hóa đơn (Bạn đã làm phần này)
            invoice.TrangThai = "Đã thanh toán";

            // 2. TẠO PHIẾU THANH TOÁN (Phần này đang thiếu hoặc chưa SaveChanges thành công)
            var vnpayMethod = _db.PhuongThucThanhToans.FirstOrDefault(pt => pt.TenPhuongThuc.Contains("VNPay"));
            
            var phieuThanhToan = new PhieuThanhToan
            {
                NgayThanhToan = DateTime.Now,
                SoTienThanhToan = invoice.TongTien,
                MaHoaDon = invoice.MaHoaDon,
                MaPhuongThuc = vnpayMethod?.MaPhuongThuc ?? 0,
                // RẤT QUAN TRỌNG: Đảm bảo gán MaKhachThue để lọc lịch sử theo từng người
                // MaKhachThue = invoice.MaKhachThue 
            };

            _db.PhieuThanhToans.Add(phieuThanhToan);
            
            // LƯU TẤT CẢ THAY ĐỔI
            _db.SaveChanges(); 

            return View("~/Views/PhieuThanhToan/ThanhtoanthanhcongVNPay.cshtml");
        }
    }
    return Content("Thanh toán không thành công");
}
    }
}
