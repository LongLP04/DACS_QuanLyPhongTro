
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
            var vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();
            var vnp_TransactionStatus = Request.Query["vnp_TransactionStatus"].ToString();

            if (response.Success && vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                // Lấy mã hóa đơn từ vnp_TxnRef để cập nhật trạng thái
                var vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();
                var invoice = _db.HoaDons.FirstOrDefault(h => h.MaHoaDon.ToString() == vnp_TxnRef);
                if (invoice != null)
                {
                    invoice.TrangThai = "Đã thanh toán";
                    // Tạo phiếu thanh toán cho giao dịch VNPay
                    var vnpayMethod = _db.PhuongThucThanhToans.FirstOrDefault(pt => pt.TenPhuongThuc.Contains("VNPay"));
                    var phieuThanhToan = new PhieuThanhToan
                    {
                        NgayThanhToan = DateTime.Now,
                        SoTienThanhToan = invoice.TongTien,
                        MaHoaDon = invoice.MaHoaDon,
                        MaPhuongThuc = vnpayMethod != null ? vnpayMethod.MaPhuongThuc : 0,
                        PhuongThucThanhToan = vnpayMethod
                    };
                    _db.PhieuThanhToans.Add(phieuThanhToan);
                    _db.SaveChanges();
                }
                // Trả về trang thanh toán thành công VNPay
                return View("~/Views/PhieuThanhToan/ThanhtoanthanhcongVNPay.cshtml");
            }
            // Nếu thất bại, hiển thị log debug
            return Content($"Thanh toán thất bại! ResponseCode: {vnp_ResponseCode}, TransactionStatus: {vnp_TransactionStatus}, OrderId: {response.OrderId}");
        }
    }
}
