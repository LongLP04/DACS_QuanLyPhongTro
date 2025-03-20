using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class ToaNha
    {
        [Key]
        public int MaToaNha { get; set; }
        public string TenToaNha { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public int TongSoTang { get; set; }
        public string? MoTa { get; set; }

        // Khóa ngoại liên kết với chủ trọ
        public int MaChuTro { get; set; }
        public ChuTro ChuTro { get; set; } = null!;

        // Danh sách phòng trọ trong tòa nhà
        public List<PhongTro> PhongTros { get; set; } = new();
    }

}
