using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal GiaDichVu { get; set; }
        public string? MoTa { get; set; }

        // Danh sách các phiếu đăng ký dịch vụ có dịch vụ này
        public ICollection<ChiTietPhieuDangKyDichVu> ChiTietPhieuDangKyDichVus { get; set; } = new List<ChiTietPhieuDangKyDichVu>();
    }

}
