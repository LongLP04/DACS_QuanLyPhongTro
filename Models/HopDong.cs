using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class HopDong
    {
        [Key]
        public int MaHopDong { get; set; }
        public DateTime NgayLap { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public string? NoiDungHopDong { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        // Khóa ngoại liên kết với Khách Thuê
        public int MaKhachThue { get; set; }
        public KhachThue KhachThue { get; set; } = null!;
    }

}
