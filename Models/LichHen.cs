using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("LichHen")]
    public class LichHen
    {
        [Key]
        public int MaLichHen { get; set; }

        [Required]
        public int MaPhong { get; set; }

        [ForeignKey("MaPhong")]
        public PhongTro PhongTro { get; set; }

        [Required]
        public int MaKhachThue { get; set; }

        [ForeignKey("MaKhachThue")]
        public KhachThue KhachThue { get; set; }

        [Required]
        public DateTime NgayHen { get; set; }

        [Required]
        public TimeSpan GioHen { get; set; }

        [Required]
        public string TrangThai { get; set; } = "Pending"; // Pending, Accepted, Rejected

        public string GhiChu { get; set; } // Ghi chú tùy chọn

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}