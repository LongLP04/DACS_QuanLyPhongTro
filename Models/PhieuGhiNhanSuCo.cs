using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class PhieuGhiNhanSuCo
    {
        [Key]
        public int MaSuCo { get; set; }
        public string MoTa { get; set; } = string.Empty;
        public DateTime NgayBaoCao { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public DateTime NgayGiaiQuyet { get; set; }

        // Khóa ngoại liên kết với Khách Thuê
        public int MaKhachThue { get; set; }
        public KhachThue KhachThue { get; set; } = null!;
    }

}
