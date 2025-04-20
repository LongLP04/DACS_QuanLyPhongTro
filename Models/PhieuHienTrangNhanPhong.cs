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

        [Required]
        public int MaKhachThue { get; set; }
        [ForeignKey("MaKhachThue")]
        public KhachThue KhachThue { get; set; }

    }
}
