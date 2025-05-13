using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("PhieuHienTrangNhanPhong")]
    public class PhieuHienTrangNhanPhong
    {
        [Key]
        public int MaPhieuHienTrang { get; set; }
        public DateTime NgayNhanPhong { get; set; }
        public string GhiChu { get; set; }
        public string TinhTrang { get; set; } = "Chưa xác nhận";

        [Required]
        public int MaKhachThue { get; set; }
        [ForeignKey("MaKhachThue")]
        public KhachThue KhachThue { get; set; }
        // Thêm thuộc tính navigation cho mối quan hệ một-nhiều
        public ICollection<HienTrangVatDung> HienTrangVatDungs { get; set; } = new List<HienTrangVatDung>();

    }
}
