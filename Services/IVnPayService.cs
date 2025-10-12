using DACS_QuanLyPhongTro.Models;
using Microsoft.AspNetCore.Http;

namespace DACS_QuanLyPhongTro.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
