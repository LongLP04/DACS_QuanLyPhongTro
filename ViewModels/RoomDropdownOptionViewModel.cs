using System.Collections.Generic;

namespace DACS_QuanLyPhongTro.ViewModels
{
    public class RoomDropdownOptionViewModel
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public string TenantName { get; set; } = "Chưa có khách";
        public List<string> RecordedReadingMonths { get; set; } = new();
        public List<string> RecordedInvoiceMonths { get; set; } = new();
    }
}
