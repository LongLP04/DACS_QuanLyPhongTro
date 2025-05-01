using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]   
        public int MaHoaDon { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TienDien { get; set; }
        public decimal TienNuoc { get; set; }
        public decimal TienPhong { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        // Khóa ngoại liên kết với Chỉ Số Điện Nước
        [Required]
        public int MaChiSo { get; set; }
        [ForeignKey("MaChiSo")]
        public ChiSoDienNuoc ChiSoDienNuoc { get; set; }

        [Required]
        public int MaKhachThue { get; set; }
        [ForeignKey("MaKhachThue")]
        public KhachThue KhachThue { get; set; }

        public ICollection<PhieuThanhToan> PhieuThanhToans { get; set; } = new List<PhieuThanhToan>();
    }
}


