namespace DACS_QuanLyPhongTro.ViewModels
{
    public class TaoPhieuHienTrangViewModel
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; }
        public string GhiChu { get; set; }
        public List<VatDungVM> VatDungs { get; set; } = new();
    }

    public class VatDungVM
    {
        public string TenVatDung { get; set; }
        public string TinhTrang { get; set; }
    }
}
