using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("PhieuThanhToan")]
    public class PhieuThanhToan
    {
        [Key]
        public int MaPhieuThanhToan { get; set; }
        public DateTime NgayThanhToan { get; set; }

        [Required]
        public decimal SoTienThanhToan { get; set; }

        [Required]
        public int MaHoaDon { get; set; }
        [ForeignKey("MaHoaDon")]
        public HoaDon HoaDon { get; set; }

        [Required]
        public int MaPhuongThuc { get; set; }
        [ForeignKey("MaPhuongThuc")]
        public PhuongThucThanhToan PhuongThucThanhToan { get; set; }
    }

}
