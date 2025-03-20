using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class ChuTro
    {
        [Key]
        public int MaChuTro { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string SoDienThoai { get; set; } = string.Empty;

        // Danh sách các tòa nhà mà chủ trọ sở hữu
        public List<ToaNha> ToaNhas { get; set; } = new();
    }

}
