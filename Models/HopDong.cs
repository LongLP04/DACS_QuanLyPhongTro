using DACS_QuanLyPhongTro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("HopDong")]
    public class HopDong
    {
        [Key]
        public int MaHopDong { get; set; }

        [Required]
        public DateTime NgayLap { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public string NoiDungHopDong { get; set; }

        [Required]
        public int MaKhachThue { get; set; }
        [ForeignKey("MaKhachThue")]
        public KhachThue KhachThue { get; set; }
        [Required]
        public int? MaPhong { get; set; }
        [ForeignKey("MaPhong")]
        public PhongTro PhongTro { get; set; }
        public string TrangThai { get; set; } = "Chờ Xác Nhận"; // Thêm thuộc tính TrangThai với giá trị mặc định
    }
}