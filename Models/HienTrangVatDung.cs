using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("HienTrangVatDung")]
    public class HienTrangVatDung
    {
        [Key]
        public int MaVatDung { get; set; }
        public string TenVatDung { get; set; }
        public string TinhTrang { get; set; }
        [Required]
        public int MaPhieuHienTrang { get; set; }
        [ForeignKey("MaPhieuHienTrang")]
        public PhieuHienTrangNhanPhong PhieuHienTrangNhanPhong { get; set; } 

    }
}
