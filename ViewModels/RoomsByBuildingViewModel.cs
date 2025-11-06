using DACS_QuanLyPhongTro.Models;

namespace DACS_QuanLyPhongTro.ViewModels
{
    public class RoomsByBuildingViewModel
    {
        public string TenToaNha { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;

        // Floors: key = floor number (int), value = list of room DTOs
        public Dictionary<int, List<RoomDto>> Floors { get; set; } = new Dictionary<int, List<RoomDto>>();
    }

    public class RoomDto
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public string? Hinhanh { get; set; }
        public decimal GiaThue { get; set; }
        public string? TrangThai { get; set; }
    }
}
