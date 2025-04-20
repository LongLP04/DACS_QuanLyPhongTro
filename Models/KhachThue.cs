using DACS_QuanLyPhongTro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("KhachThue")]
    public class KhachThue
    {
        [Key]
        public int MaKhachThue { get; set; }

        [Required]
        public string HoTenKhachThue { get; set; }

        [Required, StringLength(12)]
        public string CCCD { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string SoDienThoai { get; set; }

        [Required]
        public int MaPhong { get; set; }

        [ForeignKey("MaPhong")]
        public PhongTro PhongTro { get; set; }

        public ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();
        public ICollection<PhieuGhiNhanSuCo> PhieuGhiNhanSuCos { get; set; } = new List<PhieuGhiNhanSuCo>();
        public ICollection<PhieuThanhToan> PhieuThanhToans { get; set; } = new List<PhieuThanhToan>();
        public ICollection<PhieuDangKyDichVu> PhieuDangKyDichVus { get; set; } = new List<PhieuDangKyDichVu>();
        public ICollection<PhieuHienTrangNhanPhong> PhieuHienTrangNhanPhongs { get; set; } = new List<PhieuHienTrangNhanPhong>();
        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
