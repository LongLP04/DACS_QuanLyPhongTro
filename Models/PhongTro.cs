using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class PhongTro
    {
        [Key]
        public int MaPhong { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public int Tang { get; set; }
        public decimal DienTich { get; set; }
        public decimal GiaThue { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string HinhAnh { get; set; } = string.Empty;
        public string? MoTa { get; set; }

        // Khóa ngoại liên kết với Tòa nhà
        public int MaToaNha { get; set; }
        public ToaNha ToaNha { get; set; } = null!;

        // Liên kết với danh sách khách thuê
        public List<KhachThue> KhachThues { get; set; } = new();

        // Liên kết với chỉ số điện nước
        public List<ChiSoDienNuoc> ChiSoDienNuocs { get; set; } = new();
    }

}
