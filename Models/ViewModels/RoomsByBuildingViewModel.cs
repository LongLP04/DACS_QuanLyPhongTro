using System.Collections.Generic;

namespace DACS_QuanLyPhongTro.Models.ViewModels
{
    public class RoomsByBuildingViewModel
    {
        public int MaToaNha { get; set; }
        public string TenToaNha { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public IDictionary<int, List<PhongTroSummary>> Floors { get; set; } = new Dictionary<int, List<PhongTroSummary>>();
    }

    public class PhongTroSummary
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public decimal GiaThue { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string Hinhanh { get; set; } = string.Empty;
        // Optional tenant info
        public string KhachThueName { get; set; } = string.Empty;
        public string? KhachThueApplicationUserId { get; set; } = null;
        // Optional related entity ids for direct links
        public int? MaHopDong { get; set; }
        public int? LatestChiSoId { get; set; }
        public int? LatestHoaDonId { get; set; }
    }
}
