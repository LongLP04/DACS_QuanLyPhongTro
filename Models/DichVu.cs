using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("DichVu")]
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal DonGiaDichVu { get; set; }
        public string? MoTa { get; set; }

        // Danh sách các phiếu đăng ký dịch vụ có dịch vụ này
        public ICollection<ChiTietPhieuDangKyDichVu> ChiTietPhieuDangKyDichVus { get; set; } = new List<ChiTietPhieuDangKyDichVu>();
    }

}
